using BlockEditor.Views.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlockEditor.Models
{
    public static class OpenWindows
    {
        private static List<Window> _windows = new List<Window>();

        public static void Add(Window w)
        {
            if (w is BlockOptionWindow)
                RemoveWindows(_windows.Where(w => w is BlockOptionWindow));

            if (w is MapInfoWindow)
                RemoveWindows(_windows.Where(w => w is MapInfoWindow));

            _windows.Add(w);
        }

        public static void Remove(Window w)
        {
            _windows.Remove(w);
        }

        private static void RemoveWindows(IEnumerable<Window> windows, bool close = false)
        {
            if(windows == null)
                return;

            foreach(var w in windows)
            {
                try
                {
                    Remove(w);

                    if(close)
                        w.Close();
                }
                catch { }
            }
        }

        public static void ShowAll()
        {
            var broken = new List<Window>();

            for (int i = 0; i < _windows.Count; i++)
            {
                var w = _windows[i];

                try
                {
                    w.Activate();
                }
                catch
                {
                    broken.Add(w);
                }
            }

            RemoveWindows(broken);
        }

    }
}
