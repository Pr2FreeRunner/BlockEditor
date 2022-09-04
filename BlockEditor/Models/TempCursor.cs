using System;
using System.Windows.Input;

namespace BlockEditor.Models
{
    public class TempCursor : IDisposable
    {
        private Cursor _current;

        public TempCursor(Cursor cursor)
        {
            _current = Mouse.OverrideCursor;

            Mouse.OverrideCursor = cursor;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = _current;
        }
    }

    public class TempOverwrite : IDisposable
    {
        private bool _oldValue;
        private Blocks _blocks;

        public TempOverwrite(Blocks blocks, bool value, bool change = true)
        {
            if(blocks == null)
                return;

            _blocks  = blocks;
            _oldValue = blocks.Overwrite;

            if(change)
                _blocks.Overwrite = value;
        }

        public void Dispose()
        {
            if(_blocks == null)
                return;

            _blocks.Overwrite = _oldValue;
        }
    }
}
