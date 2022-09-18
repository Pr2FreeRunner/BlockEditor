using System;
using System.IO;
using System.Windows.Input;
using BlockEditor.Helpers;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

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
                RaisePropertyChanged(nameof(IsBuildMode));
                RaisePropertyChanged(nameof(IsTransformMode));
                RaisePropertyChanged(nameof(IsDeleteMode));
                RaisePropertyChanged(nameof(IsEditMode));
                RaisePropertyChanged(nameof(IsAdvancedMode));
                RaisePropertyChanged(nameof(IsInfoMode));

                SetMouseCursor(value);
                ClearSidePanel(value);
            }
        }


        public bool IsSelectionMode => Value == UserModes.Selection;
        public bool IsBuildMode => Value == UserModes.Fill;
        public bool IsTransformMode => false;
        public bool IsDeleteMode => Value == UserModes.Delete;
        public bool IsEditMode => false;
        public bool IsAdvancedMode => Value == UserModes.Distance || Value == UserModes.ConnectTeleports;
        public bool IsInfoMode => Value == UserModes.BlockInfo;


        public void UpdateGuiState()
        {
            Value = Value;
        }

        public bool UsesSidePanel()
        {
            if(Value == UserModes.ConnectTeleports)
                return true;

            return false;
        }

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

        private void ClearSidePanel(UserModes mode)
        {
            if (mode == UserModes.ConnectTeleports)
                return;

            App.MyMainWindow?.CurrentMap?.ClearSidePanel();
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
