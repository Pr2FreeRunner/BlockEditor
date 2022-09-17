using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
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
using static BlockEditor.Models.MySearch;

namespace BlockEditor.Views.Windows
{

    public partial class LoadMapWindow : Window
    {

        private SearchBy _searchBy;
        private bool _initDone;
        private bool _disableSearch;
        private int _page = 1;
        private const string NOT_FOUND = "404 Not Found";
        private static readonly MySearch _lastSearch = new MySearch();
        public int SelectedLevelID { get; private set; }
        public Level SelectedLevel { get; private set; }
        public bool Unpublish { get; private set; }

        public LoadMapWindow() 
        {
            _initDone = false;
            Unpublish = true;
            InitializeComponent();
            AddSearchByItems();
            AddSearchModeItems();
            AddSearchOrderItems();
            UpdateButtons();
            Clean();

            OpenWindows.Add(this);
            _disableSearch = false;
            _initDone = true;
        }


        private void AddSearchModeItems()
        {
            foreach (SearchDirectionEnum type in Enum.GetValues(typeof(SearchDirectionEnum)))
            {
                var item = new ComboBoxItem();
                item.Content = MyUtil.InsertSpaceBeforeCapitalLetter(type.ToString());
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
                item.Content = MyUtil.InsertSpaceBeforeCapitalLetter(type.ToString());
                OrderComboBox.Items.Add(item);
            }

            if (OrderComboBox.Items != null && OrderComboBox.Items.Count > 0)
                OrderComboBox.SelectedIndex = 0;
        }

        private void AddSearchByItems()
        {
            foreach (SearchBy type in Enum.GetValues(typeof(SearchBy)))
            {
                if(type == SearchBy.MyLevels && !Users.IsLoggedIn())
                    continue;

                if (type == SearchBy.GetLastSearch && !_lastSearch.IsValid())
                    continue;

                var item = new ComboBoxItem();
                var name = type == SearchBy.BestWeek ? "Week's Best" : type.ToString();
                var hotkey = type == SearchBy.GetLastSearch ? 'S' : name.First();

                item.ToolTip = "HotKey:  Ctrl + " + hotkey;
                item.Content = MyUtil.InsertSpaceBeforeCapitalLetter(name);
                item.Tag = type;

                SearchByComboBox.Items.Add(item);
            }

            if (SearchByComboBox.Items != null && SearchByComboBox.Items.Count > 0)
                SearchByComboBox.SelectedIndex = 0;
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
            var pageOk     = (okToSearch && (_searchBy == SearchBy.Username || _searchBy == SearchBy.Title)) ||  _searchBy == SearchBy.Newest || _searchBy == SearchBy.BestWeek;
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

            if(_searchBy != SearchBy.GetLastSearch 
                && _searchBy != SearchBy.MyLevels 
                && _searchBy != SearchBy.LocalFile 
                && _initDone)
            {
                _lastSearch.Page = _page;
                _lastSearch.Order = info.Order;
                _lastSearch.Direction = info.Direction;
                _lastSearch.SearchValue = value;
                _lastSearch.SearchType = _searchBy;
                _lastSearch.Save();
            }

            return info;
        }


        #region Events

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (_disableSearch)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                try
                {
                    var searchInfo = GetSearchInfo();

                    Clean();

                    switch (_searchBy)
                    {
                        case SearchBy.Title:
                        case SearchBy.Username:
                            if(string.IsNullOrWhiteSpace(searchInfo.SearchValue))
                                return;

                            AddSearchResults(SearchLevel(searchInfo));
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

                        case SearchBy.BestWeek:
                            AddSearchResults(SearchBestWeek(_page));
                            break;

                        case SearchBy.GetLastSearch:
                            if (!_lastSearch.IsValid())
                                break;

                            _disableSearch = true;
                            OrderComboBox.SelectedIndex = (int)_lastSearch.Order;
                            SearchDirectionComobBox.SelectedIndex = (int)_lastSearch.Direction;
                            searchTextbox.Text = _lastSearch.SearchValue;
                            SearchByComboBox.SelectedItem = GetSearchByItem(_lastSearch.SearchType);
                            _page = _lastSearch.Page;
                            _disableSearch = false;
                            Search_Click(null, null);
                            break;

                        default: throw new Exception("Unknown search config....");
                    }
                }
                catch (Exception ex)
                {
                    if (!MyUtil.HasInternet())
                        MessageUtil.ShowError("Failed to load levels, check ur internet connection...");
                    else
                        MessageUtil.ShowError(ex.Message);
                }
            }

            UpdateButtons();
        }
 
        private void SearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = SearchByComboBox.SelectedItem as ComboBoxItem;

                if(item == null)
                    return;

                _page = 1;
                _searchBy = (SearchBy)item.Tag;
                Clean();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

            UpdateButtons();
            searchTextbox.Focus();


            if (_searchBy == SearchBy.MyLevels 
                         || _searchBy == SearchBy.LocalFile
                         || _searchBy == SearchBy.Newest
                         || _searchBy == SearchBy.BestWeek
                         || _searchBy == SearchBy.GetLastSearch)
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

        private ComboBoxItem GetSearchByItem(SearchBy type)
        {
            if(SearchByComboBox?.Items == null)
                return null;

            foreach(var child in SearchByComboBox?.Items)
            {
                var item = child as ComboBoxItem;

                if(item?.Tag == null)
                    continue;

                if(((SearchBy) item.Tag) == type)
                    return item;
            }

            return null;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            
            if(ctrl && e.Key == Key.M && Users.IsLoggedIn())
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.MyLevels);
            }
            else if (ctrl && e.Key == Key.N)
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.Newest);
            }
            else if (ctrl && e.Key == Key.T)
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.Title);
            }
            else if (ctrl && e.Key == Key.W)
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.BestWeek);
            }
            else if (ctrl && e.Key == Key.U)
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.Username);
            }
            else if (ctrl && e.Key == Key.I)
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.ID);
            }
            else if (ctrl && e.Key == Key.S && _lastSearch.IsValid())
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.GetLastSearch);
            }
            else if (ctrl && e.Key == Key.L)
            {
                SearchByComboBox.SelectedItem = GetSearchByItem(SearchBy.LocalFile);
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
            else if (ctrl && e.Key == Key.Left && btnLeftPage.IsEnabled)
            {
                OnPreviousPage(null, null);
            }
            else if (ctrl && e.Key == Key.Right && btnRightPage.IsEnabled)
            {
                OnNextPage(null, null);
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
            Unpublish = _searchBy != SearchBy.MyLevels;

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

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchTextbox.Focus();
            Search_Click(null, null);
        }

        #endregion


    }
}
