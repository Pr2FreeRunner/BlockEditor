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
        }

        private void btnTitle_Click(object sender, RoutedEventArgs e)
        {
            OnSelectedLevel?.Invoke(_id);
        }
    }
}
