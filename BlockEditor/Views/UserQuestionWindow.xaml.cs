using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlockEditor.Views
{
    public partial class UserQuestionWindow : Window
    {
        public enum QuestionResult { Cancel, Yes, No }

        private QuestionResult _result;

        public UserQuestionWindow(string text, string title, bool showCancel)
        {
            InitializeComponent();
            txtQuestion.Text = text;
            this.Title = title;
            btnCancel.Visibility = showCancel ? Visibility.Visible : Visibility.Collapsed;
            _result = QuestionResult.Cancel;
        }

        public static QuestionResult Show(string text, string title, bool showCancel = true)
        {
            var current = Mouse.OverrideCursor;
            try
            {
                Mouse.OverrideCursor = null;

                var w = new UserQuestionWindow(text, title, showCancel);

                w.ShowDialog();

                return w._result;
            }
            finally
            {
                Mouse.OverrideCursor = current;
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.Yes;
            Close();
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.No;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.Cancel;
            Close();
        }

    }
}
