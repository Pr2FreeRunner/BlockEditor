using System;
using System.IO;
using System.Windows.Input;
using BlockEditor.Helpers;

namespace BlockEditor.Models
{

    public class UserMode : NotificationObject
    {
        public enum UserModes { None, AddBlock, Selection, AddSelection, Fill, BlockInfo, MapInfo, BlockCount }

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
                RaisePropertyChanged(nameof(IsMapInfoMode));
                RaisePropertyChanged(nameof(IsBlockCountMode));

                SetMouseCursor(value);
            }
        }

     
        public bool IsSelectionMode => Value == UserModes.Selection;
        public bool IsFillMode => Value == UserModes.Fill;
        public bool IsBlockInfoMode => Value == UserModes.BlockInfo;
        public bool IsBlockCountMode => Value == UserModes.BlockCount;
        public bool IsMapInfoMode => Value == UserModes.MapInfo;

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

                    case UserModes.BlockInfo:
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
