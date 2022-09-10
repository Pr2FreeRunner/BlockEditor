using BlockEditor.Models;
using BlockEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BlockEditor
{

    public partial class App : Application
    {
        public App()
        {
        }

        public static MainWindow MyMainWindow
        {
            get
            {
                return App.Current.MainWindow as MainWindow;
            }
        }
    }
}
