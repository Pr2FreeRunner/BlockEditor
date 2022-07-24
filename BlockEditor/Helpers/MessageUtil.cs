using LevelModel.DTO;
using System.Windows;
using static LevelModel.DTO.Message;

namespace BlockEditor.Helpers
{
    public static class MessageUtil
    {

        public static void ShowError(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(string msg)
        {
            MessageBox.Show(msg, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string msg)
        {
            MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        public static void ShowMessage(Message msg)
        {
            if (msg == null)
                return;

            switch (msg.Type)
            {
                case MessageType.Warning:
                    ShowWarning(msg.Content);
                    break;

                case MessageType.Normal:
                    ShowError(msg.Content);
                    break;

                default:
                    ShowError(msg.Content);
                    break;
            }
        }

        public static void ShowMessages(Messages messages)
        {
            if(messages == null)
                return;

            foreach (var msg in messages)
                ShowMessage(msg);
        }

    }
}
