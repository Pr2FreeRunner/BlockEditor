using BlockEditor.Models;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BlockEditor.Views.Windows
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


            OpenWindows.Add(this);
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

        public static QuestionResult ShowWarning(string text, bool showCancel = true)
        {
            var current = Mouse.OverrideCursor;
            try
            {
                Mouse.OverrideCursor = null;

                var w = new UserQuestionWindow(text, "Warning", showCancel);
                w.MyBorder.BorderBrush = new SolidColorBrush(Colors.Yellow);

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                _result = QuestionResult.Cancel;
                Close();
            }
        }
    }
}
