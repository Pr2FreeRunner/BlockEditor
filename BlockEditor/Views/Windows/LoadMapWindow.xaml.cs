using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Controls;
using DataAccess.DataStructures;
using LevelModel.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static BlockEditor.Utils.SearchLevelUtil;
using static DataAccess.DataStructures.SearchLevelInfo;

namespace BlockEditor.Views.Windows
{

    public partial class LoadMapWindow : Window
    {
        private enum SearchBy { Username, Title, ID, LocalFile, Newest, MyLevels } 

        private SearchBy _searchBy;
        private int _page = 1;
        private string NOT_FOUND = "404 Not Found";

        public int SelectedLevelID { get; private set; }
        public Level SelectedLevel { get; private set; }


        public LoadMapWindow() 
        {
            InitializeComponent();
            AddSearchByItems();
            AddSearchModeItems();
            AddSearchOrderItems();
            UpdateButtons();
            Clean();

            OpenWindows.Add(this);
        }

        private void AddSearchModeItems()
        {
            foreach (SearchDirectionEnum type in Enum.GetValues(typeof(SearchDirectionEnum)))
            {
                var item = new ComboBoxItem();
                item.Content = InsertSpaceBeforeCapitalLetter(type.ToString());
                SearchDirectionComobBox.Items.Add(item);
            }

            if (SearchDirectionComobBox.Items != null && SearchDirectionComobBox.Items.Count > 0)
                SearchDirectionComobBox.SelectedIndex = 0;
        }

        private void AddSearchOrderItems()
        {
            foreach (SearchOrderEnum type in Enum.GetValues(typeof(SearchOrderEnum)))
            {
                var item = new ComboBoxItem();
                item.Content = InsertSpaceBeforeCapitalLetter(type.ToString());
                OrderComboBox.Items.Add(item);
            }

            if (OrderComboBox.Items != null && OrderComboBox.Items.Count > 0)
                OrderComboBox.SelectedIndex = 0;
        }

        private void AddSearchByItems()
        {
            foreach (SearchBy type in Enum.GetValues(typeof(SearchBy)))
            {
                if(type == SearchBy.MyLevels && CurrentUser.IsLoggedIn() == false)
                    continue;

                var item     = new ComboBoxItem();
                item.ToolTip = "HotKey:  Ctrl + " + type.ToString().First();
                item.Content = InsertSpaceBeforeCapitalLetter(type.ToString());
                SearchByComboBox.Items.Add(item);
            }

            if (SearchByComboBox.Items != null && SearchByComboBox.Items.Count > 0)
                SearchByComboBox.SelectedIndex = 0;
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
            var okToSearch = IsOKToSearch();
            var pageOk     = (okToSearch && _searchBy == SearchBy.Username) ||  _searchBy == SearchBy.Newest;
            var fullSearch = _searchBy == SearchBy.Username || _searchBy == SearchBy.Title;

            btnSearch.IsEnabled     = okToSearch;
            btnRightPage.IsEnabled  = pageOk;
            btnLeftPage.IsEnabled   = pageOk && _page > 1;
            searchTextbox.IsEnabled = _searchBy != SearchBy.MyLevels 
                                    && _searchBy != SearchBy.LocalFile
                                    && _searchBy != SearchBy.Newest;

            PageText.Text = _page.ToString(CultureInfo.InvariantCulture);

            SearchDirectionComobBox.IsEnabled  = fullSearch;
            OrderComboBox.IsEnabled = fullSearch;
        }

        private SearchLevelInfo GetSearchInfo()
        {
            var value = searchTextbox.Text;
            var info  = new SearchLevelInfo(value, _page);

            info.Order     = (SearchOrderEnum)OrderComboBox.SelectedIndex;
            info.Direction = (SearchDirectionEnum)SearchDirectionComobBox.SelectedIndex;

            switch (_searchBy)
            {
                case SearchBy.Username:
                    info.Mode = SearchModeEnum.User;
                    break;

                case SearchBy.Title:
                    info.Mode = SearchModeEnum.Title;
                    break;
            }

            return info;
        }

        #region Events

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            using (new TempCursor(Cursors.Wait))
            {
                try
                {
                    Clean();
                    var searchInfo = GetSearchInfo();

                    switch (_searchBy)
                    {
                        case SearchBy.Title:
                        case SearchBy.Username:
                            AddSearchResults(Search(searchInfo));
                            break;

                        case SearchBy.ID:
                            var id = GetLevelID(searchInfo.SearchValue);

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
                        default: throw new Exception("Unknown search config....");
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
            else if (ctrl && e.Key == Key.Enter && SearchResultPanel.Children != null  && SearchResultPanel.Children.Count > 0)
            {
                var mapControl  = SearchResultPanel.Children[0] as SearchResultControl;

                if(mapControl == null)
                    return;

                mapControl.InvokeSelectedLevel();
            }
            else if (!ctrl && e.Key == Key.Enter && searchTextbox.IsFocused && IsOKToSearch())
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

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
