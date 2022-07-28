using BlockEditor.Helpers;
using BlockEditor.Models;
using DataAccess;
using Newtonsoft.Json.Linq;
using Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views
{

    public partial class LoadMapWindow : Window
    {
        private enum SearchBy { Username, ID, MyLevels }

        public int SelectedLevelID { get; private set; }

        private SearchBy _searchBy;

        private int _page = 1;
        private string NOT_FOUND = "404 Not Found";

        public LoadMapWindow()
        {
            InitializeComponent();
            AddSearchByItems();
            UpdateButtons();
        }

        private void AddSearchByItems()
        {
            Clean();

            foreach (SearchBy type in Enum.GetValues(typeof(SearchBy)))
            {
                if(type == SearchBy.MyLevels && CurrentUser.IsLoggedIn() == false)
                    continue;

                var item     = new ComboBoxItem();
                item.Content = InsertSpaceBeforeCapitalLetter(type.ToString());
                SearchByComboBox.Items.Add(item);
            }
        }

        private string InsertSpaceBeforeCapitalLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            if (string.Equals("ID", input, StringComparison.InvariantCultureIgnoreCase))
                return input;

            return string.Concat(input.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }

        private void AddSearchResults(IEnumerable<SearchResult> results)
        {
            if (results == null)
            {
                errorText.Content = NOT_FOUND;
                return;
            }

            var any = false;
            foreach (var item in results)
            {
                if (item == null)
                    continue;

                any = true;

                if(item == SearchResult.SLOW_DOWN)
                {
                    errorText.Content = "Slow down a bit, yo.";
                    continue;
                }

                var control = new SearchResultControl(item);
                control.OnSelectedLevel += OnSelectedLevel;

                SearchResultPanel.Children.Add(control);
            }

            if (!any)
                errorText.Content = NOT_FOUND;
        }

        private void OnSelectedLevel(int id)
        {
            SelectedLevelID = id;
            DialogResult = true;
            Close();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            using (new TempCursor(Cursors.Wait))
            {
                try
                {
                    Clean();
                    var search = searchTextbox.Text;

                    switch (_searchBy)
                    {
                        case SearchBy.Username:
                            AddSearchResults(SearchByUsername(search));
                            break;

                        case SearchBy.ID:
                            var id = GetLevelID(search);

                            if (id == null)
                                errorText.Content = "Invalid Level ID";
                            else
                                AddSearchResults(SearchByLevelId(id.Value));
                            break;

                        case SearchBy.MyLevels:
                            AddSearchResults(SearchMyLevels());
                            break;

                        default: throw new Exception("Something is wrong...");
                    }
                }
                catch (Exception ex)
                {
                    MessageUtil.ShowError(ex.Message);
                }
            }
        }

        private bool IsSlowDownResponse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return false;

            try
            {
                var json = JObject.Parse(data);
                var success = json?.GetValue("success")?.Value<bool>() ?? false;
                var msg = json?.GetValue("error")?.Value<string>() ?? string.Empty;

                return !success && msg != null && msg.Contains("Slow down", StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private IEnumerable<SearchResult> SearchByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                yield break;

            var data = PR2Accessor.Search(username, _page);

            if (IsSlowDownResponse(data))
            {
                yield return SearchResult.SLOW_DOWN;
                yield break;
            }

            var levels = PR2Parser.SearchResult(data);

            foreach (var l in levels)
            {
                if (l == null)
                    continue;

                yield return new SearchResult(l.LevelID, l.Title);
            }
        }

        private IEnumerable<SearchResult> SearchMyLevels()
        {
            if (!CurrentUser.IsLoggedIn())
            {
                MessageUtil.ShowError("Requires user to login");
                yield break;
            }

            var data = PR2Accessor.LoadMyLevels(CurrentUser.Token);

            if (IsSlowDownResponse(data))
            {
                yield return SearchResult.SLOW_DOWN;
                yield break;
            }

            var levels = PR2Parser.LoadResult(data);

            foreach (var l in levels)
            {
                if (l == null)
                    continue;

                yield return new SearchResult(l.LevelID, l.Title);
            }
        }

        private int? GetLevelID(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (!MyConverters.TryParse(input, out var id))
                return null;

            return id;
        }

        private IEnumerable<SearchResult> SearchByLevelId(int id)
        {
            SearchResult result = null;

            try
            {
                var data = PR2Accessor.Download(id);

                if (string.IsNullOrWhiteSpace(data))
                    yield break;

                if (IsSlowDownResponse(data))
                {
                    result = SearchResult.SLOW_DOWN;
                }
                else
                {
                    var levelInfo = PR2Parser.Level(data);

                    if (levelInfo?.Level?.Title == null)
                    {
                        MessageUtil.ShowError("Failed to parse level");
                        yield break;
                    }

                    result = new SearchResult(levelInfo.Level.LevelID, levelInfo.Level.Title);
                }
            }
            catch (WebException ex)
            {
                var r = ex.Response as HttpWebResponse;

                if (r != null && r.StatusCode == HttpStatusCode.NotFound)
                    yield break;
                else
                    throw;
            }

            if (result != null)
                yield return result;
        }

        private void Clean()
        {
            errorText.Content = string.Empty;
            SearchResultPanel.Children.Clear();
        }

        private void SearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _searchBy = (SearchBy)SearchByComboBox.SelectedIndex;
                Clean();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

            UpdateButtons();

            if (_searchBy == SearchBy.MyLevels)
            {
                searchTextbox.Text = string.Empty;
                Search_Click(null, null);
            }
        }

        private bool IsOKToSearch()
        {
            return !string.IsNullOrWhiteSpace(searchTextbox.Text) || _searchBy == SearchBy.MyLevels;
        }

        private void UpdateButtons()
        {
            var ok = IsOKToSearch();
            var pageOk = ok && _searchBy == SearchBy.Username;

            btnSearch.IsEnabled = ok;
            btnRightPage.IsEnabled = pageOk;
            btnLeftPage.IsEnabled = pageOk && _page > 1;
            searchTextbox.IsEnabled = _searchBy != SearchBy.MyLevels;

            PageText.Text = _page.ToString(CultureInfo.InvariantCulture);
        }

        private void searchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorText.Content = string.Empty;
            UpdateButtons();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsOKToSearch())
                return;

            if (e.Key == Key.Enter)
                Search_Click(null, null);

            UpdateButtons();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            searchTextbox.Focus();
        }

        private void OnPreviousPage(object sender, RoutedEventArgs e)
        {
            try
            {
                _page--;

                Search_Click(null, null);

                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

        }

        private void OnNextPage(object sender, RoutedEventArgs e)
        {
            try
            {
                _page++;

                Search_Click(null, null);

                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }
    }
}
