using LevelModel.DTO;
using System;
using System.Windows;
using System.Windows.Input;
using static LevelModel.DTO.Message;

namespace BlockEditor.Helpers
{
    public static class MessageUtil
    {

        public static void UseDefoutMouse(Action a)
        {
            if(a  == null)
                return;

            var current = Mouse.OverrideCursor;

            try
            {
                Mouse.OverrideCursor = null;
                a?.Invoke();
            }
            finally
            {
                Mouse.OverrideCursor = current;
            }
        }

        public static void ShowError(string msg)
        {
            UseDefoutMouse(() => MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        public static void ShowInfo(string msg)
        {
            UseDefoutMouse(() => MessageBox.Show(msg, "Info", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        public static void ShowWarning(string msg)
        {
            UseDefoutMouse(() => MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));
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
