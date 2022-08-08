using System.Collections.Generic;
using System.Windows;

namespace BlockEditor.Models
{
    public static class OpenWindows
    {
        private static List<Window> _windows = new List<Window>();

        public static void Add(Window w)
        {
            _windows.Add(w);
        }

        public static void Remove(Window w)
        {
            _windows.Remove(w);
        }

        private static void RemoveBrokenWindows(List<Window> windows)
        {
            if(windows == null)
                return;

            foreach(var w in windows)
            {
                try
                {
                    Remove(w);
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

            RemoveBrokenWindows(broken);
        }

    }
}
