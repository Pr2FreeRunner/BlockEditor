using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Controls;
using LevelModel.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static BlockEditor.Utils.SearchLevelUtil;

namespace BlockEditor.Views.Windows
{

    public partial class LoadMapWindow : Window
    {
        private enum SearchBy { Username, ID, LocalFile, Newest, MyLevels } 

        private SearchBy _searchBy;
        private int _page = 1;
        private string NOT_FOUND = "404 Not Found";

        public int SelectedLevelID { get; private set; }
        public Level SelectedLevel { get; private set; }


        public LoadMapWindow() 
        {
            InitializeComponent();
            AddSearchByItems();
            UpdateButtons();

            OpenWindows.Add(this);
        }


        private void AddSearchByItems()
        {
            Clean();

            foreach (SearchBy type in Enum.GetValues(typeof(SearchBy)))
            {
                if(type == SearchBy.MyLevels && CurrentUser.IsLoggedIn() == false)
                    continue;

                if (type == SearchBy.Newest && CurrentUser.IsLoggedIn() == false)
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
            {
                errorText.Content = NOT_FOUND;
            }
        }

        private void Close(bool success)
        {
            DialogResult = success;
            Close();
        }

        private void Clean()
        {
            errorText.Content = string.Empty;
            SearchResultPanel.Children.Clear();
        }

        private bool IsOKToSearch()
        {
            return !string.IsNullOrWhiteSpace(searchTextbox.Text) || _searchBy == SearchBy.LocalFile;
        }

        private void UpdateButtons()
        {
            var ok     = IsOKToSearch();
            var pageOk = (ok && _searchBy == SearchBy.Username) ||  _searchBy == SearchBy.Newest;

            btnSearch.IsEnabled = ok;
            btnRightPage.IsEnabled = pageOk;
            btnLeftPage.IsEnabled  = pageOk && _page > 1;
            searchTextbox.IsEnabled = _searchBy != SearchBy.MyLevels 
                                    && _searchBy != SearchBy.LocalFile
                                    && _searchBy != SearchBy.Newest;

            PageText.Text = _page.ToString(CultureInfo.InvariantCulture);
        }

        #region Events
        
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
                            AddSearchResults(SearchByUsername(search, _page));
                            break;

                        case SearchBy.ID:
                            var id = GetLevelID(search);

                            if (id == null)
                                errorText.Content = "Invalid Level ID";
                            else
                                AddSearchResults(SearchByLevelId(id.Value, Close, OnSelectedLevel));
                            break;

                        case SearchBy.MyLevels:
                            AddSearchResults(SearchMyLevels());
                            break;

                        case SearchBy.LocalFile:
                            AddSearchResults(SearchLocalFile(Close, OnSelectedLevel));
                            break;

                        case SearchBy.Newest:
                            AddSearchResults(SearchNewest(_page));
                            break;
                        default: throw new Exception("Something is wrong...");
                    }
                }
                catch (Exception ex)
                {
                    MessageUtil.ShowError(ex.Message);
                }
            }

            UpdateButtons();
        }
 
        private void SearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _page = 1;
                _searchBy = (SearchBy)SearchByComboBox.SelectedIndex;
                Clean();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

            UpdateButtons();

            if (_searchBy == SearchBy.MyLevels 
                         || _searchBy == SearchBy.LocalFile
                         || _searchBy == SearchBy.Newest)
            {
                searchTextbox.Text = string.Empty;
                Search_Click(null, null);
            }
        }

        private void searchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorText.Content = string.Empty;
            UpdateButtons();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            
            if(ctrl && e.Key == Key.M)
            {
                SearchByComboBox.SelectedIndex = (int)SearchBy.MyLevels;
            }
            else if (ctrl && e.Key == Key.N)
            {
                SearchByComboBox.SelectedIndex = (int)SearchBy.Newest;
            }
            else if (ctrl && e.Key == Key.U)
            {
                SearchByComboBox.SelectedIndex = (int)SearchBy.Username;
            }
            else if (ctrl && e.Key == Key.I)
            {
                SearchByComboBox.SelectedIndex = (int)SearchBy.ID;
            }
            else if (ctrl && e.Key == Key.L)
            {
                SearchByComboBox.SelectedIndex = (int)SearchBy.LocalFile;
            }
            else if (e.Key == Key.Enter && searchTextbox.IsFocused && IsOKToSearch())
            {
                Search_Click(null, null);
                e.Handled = true;
            }

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

        private void OnSelectedLevel(int id)
        {
            SelectedLevelID = id;
            Close(true);
        }

        private void OnSelectedLevel(Level level)
        {
            SelectedLevel = level;
            OnSelectedLevel(level.LevelID);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        #endregion


    }
}
