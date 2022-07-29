using BlockEditor.Views.Windows;
using LevelModel.DTO;

using static LevelModel.DTO.Message;

namespace BlockEditor.Helpers
{
    public static class MessageUtil
    {

        public static void ShowError(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                return;

            MessageWindow.Show(msg, MessageWindow.MessageWindowType.Error);
        }

        public static void ShowInfo(string msg)
        {
            if(string.IsNullOrWhiteSpace(msg))
                return;

            MessageWindow.Show(msg, MessageWindow.MessageWindowType.Info);
        }

        public static void ShowWarning(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                return;

            MessageWindow.Show(msg, MessageWindow.MessageWindowType.Warning);
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
