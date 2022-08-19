using BlockEditor.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BlockEditor.Views.Controls
{

    public partial class SearchResultControl : UserControl
    {

        public event Action<int> OnSelectedLevel;

        private int _id;

        public SearchResultControl(SearchResult result)
        {
            InitializeComponent();

            if (result == null)
                throw new ArgumentNullException("level");

            _id = result.ID;
            btnTitle.Content = result.Title;
            btnTitle.ToolTip = "Created By:  " +  (result.CreatedBy ?? string.Empty);
        }

        private void btnTitle_Click(object sender, RoutedEventArgs e)
        {
            OnSelectedLevel?.Invoke(_id);
        }

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                OnSelectedLevel?.Invoke(_id);
        }
    }
}
