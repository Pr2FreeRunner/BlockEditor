using BlockEditor.Helpers;
using BlockEditor.Views.Windows;
using System;
using System.IO;
using System.Windows;

namespace BlockEditor
{

    public partial class App : Application
    {
        public App()
        {
#if !DEBUG
            App.Current.DispatcherUnhandledException += OnUnhandledException;
#endif
        }

        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageUtil.ShowError(e.Exception.Message);
            var crashDump = e.Exception.Message + ":" + Environment.NewLine + e.Exception.StackTrace;
            File.WriteAllText("crash-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt", crashDump);
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
