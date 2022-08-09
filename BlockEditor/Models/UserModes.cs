
using BlockEditor.Helpers;
using System.Windows.Input;

namespace BlockEditor.Models
{

    public class UserMode : NotificationObject
    {
        public enum UserModes { None, AddBlock, Selection, AddSelection, Fill, BlockInfo, MapInfo, BlockCount }


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


                if (_value != UserModes.Fill || _value != UserModes.BlockInfo)
                    Mouse.OverrideCursor = null;
            }
        }

        public bool IsSelectionMode => Value == UserModes.Selection;
        public bool IsFillMode => Value == UserModes.Fill;
        public bool IsBlockInfoMode => Value == UserModes.BlockInfo;
        public bool IsBlockCountMode => Value == UserModes.BlockCount;
        public bool IsMapInfoMode => Value == UserModes.MapInfo;

        public UserMode()
        {
            Value = UserModes.None;
        }
    }
}
