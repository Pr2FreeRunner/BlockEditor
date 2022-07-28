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
}
