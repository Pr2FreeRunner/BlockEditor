using System;
using System.IO;
using System.Windows.Input;
using BlockEditor.Helpers;

namespace BlockEditor.Models
{

    public class UserMode : NotificationObject
    {
        public enum UserModes { None, Selection, Fill, BlockInfo, ConnectTeleports, MoveBlock, GetPosition, Distance, Delete }

        private static Cursor BucketCursor;

        private UserModes _value;

        public UserModes Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                RaisePropertyChanged(nameof(IsSelectionMode));
                RaisePropertyChanged(nameof(IsFillMode));
                RaisePropertyChanged(nameof(IsBlockInfoMode));
                RaisePropertyChanged(nameof(IsConnectTeleportsMode));
                RaisePropertyChanged(nameof(IsDistanceMode));
                RaisePropertyChanged(nameof(IsDeleteMode));


                SetMouseCursor(value);
            }
        }

     
        public bool IsSelectionMode => Value == UserModes.Selection;
        public bool IsFillMode => Value == UserModes.Fill;
        public bool IsBlockInfoMode => Value == UserModes.BlockInfo;
        public bool IsConnectTeleportsMode => Value == UserModes.ConnectTeleports;
        public bool IsDistanceMode => Value == UserModes.Distance;
        public bool IsDeleteMode => Value == UserModes.Delete;




        public static void Init()
        {
            try
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Cursors");
                BucketCursor = new Cursor(Path.Combine(folder, "BucketCursor.cur"));
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void SetMouseCursor(UserModes mode)
        {
            try
            {
                switch (mode)
                {
                    case UserModes.Selection:
                        Mouse.OverrideCursor = Cursors.Hand;
                        break;

                    case UserModes.Fill:
                        Mouse.OverrideCursor = BucketCursor;
                        break;

                    case UserModes.GetPosition:
                    case UserModes.BlockInfo:
                        Mouse.OverrideCursor = Cursors.Cross;
                        break;

                    case UserModes.ConnectTeleports:
                        Mouse.OverrideCursor = Cursors.Cross;
                        break;

                    default:
                        Mouse.OverrideCursor = null;
                        break;
                }
            }
            catch
            {
                Mouse.OverrideCursor = null;
            }
        }


        public UserMode()
        {
            Value = UserModes.None;
        }
    }
}
