using BlockEditor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BlockEditor
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

        public static void ShowAll()
        {
            var count = _windows.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                var w = _windows[i];

                w.Activate();
            }
        }
    }

    public partial class App : Application
    {


        public App()
        {
            BlockImages.Init();
        }


    }
}
