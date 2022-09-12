using BlockEditor.Views.Windows;
using System;
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

        public static bool IsSidePanelActive()
        {
            return MyMainWindow?.CurrentMap?.GetSidePanel() != null;
        }
    }
}
