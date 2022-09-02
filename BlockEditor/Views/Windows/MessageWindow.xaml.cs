using BlockEditor.Models;
using System.Windows;
using System.Windows.Media;

namespace BlockEditor.Views.Windows
{
    public partial class MessageWindow : Window
    {
        public enum MessageWindowType { Info, Warning , Error }

        private MessageWindow(string msg, MessageWindowType type)
        {
            InitializeComponent();

            msgText.Text = msg;

            if (type == MessageWindowType.Warning)
            {
                MsgBorder.BorderBrush = new SolidColorBrush(Colors.Gold);
                this.Title = "Warning";
            }
            else if(type == MessageWindowType.Error)
            {
                MsgBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                this.Title = "Error";
            }
            else
            {
                this.Title = "Info";
            }


            OpenWindows.Add(this);
        }

        public static void Show(string msg, MessageWindowType type)
        {
            if(string.IsNullOrWhiteSpace(msg))
                return;

            using(new TempCursor(null))
            {
                new MessageWindow(msg, type).ShowDialog();
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Escape)
                Close();
        }
    }
}
