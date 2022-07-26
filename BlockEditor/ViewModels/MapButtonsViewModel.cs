using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views;
using DataAccess;
using Parsers;

namespace BlockEditor.ViewModels
{
    public class MapButtonsViewModel : NotificationObject
    {

        public event Action<Map> OnLoadMap;
        public event Action OnSaveMap;



        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand NewCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }


        public MapButtonsViewModel()
        {
            LoadCommand = new RelayCommand(LoadExecute);
            SaveCommand = new RelayCommand(SaveExecute, CanSave);
            NewCommand = new RelayCommand(NewExecute);
            TestCommand = new RelayCommand(TestExecute);
            LoginCommand = new RelayCommand(LoginExecute);

        }

        private bool CanSave(object obj)
        {
            return CurrentUser.IsLoggedIn();
        }

        private void LoginExecute(object obj)
        {
            new LoginWindow().ShowDialog();
        }

        private void SaveExecute(object obj)
        {
            OnSaveMap?.Invoke();
        }

        private void LoadExecute(object obj)
        {
            try
            {
                var input = UserInputWindow.Show("Level ID: ", "Input");
                Mouse.OverrideCursor = Cursors.Wait;

                if (string.IsNullOrWhiteSpace(input))
                    return;

                if (!MyConverters.TryParse(input, out var id))
                {
                    MessageUtil.ShowError("Invalid Level ID");
                    return;
                }

                var data = PR2Accessor.Download(id);

                if (string.IsNullOrWhiteSpace(data))
                {

                    MessageUtil.ShowError("Failed to download level");
                    return;
                }

                var levelInfo = PR2Parser.Level(data);

                if (levelInfo == null)
                {
                    MessageUtil.ShowError("Failed to parse level");
                    return;
                }

                if (levelInfo.Messages == null || levelInfo.Messages.Any())
                {
                    MessageUtil.ShowMessages(levelInfo.Messages);
                    return;
                }

                var map = new Map(levelInfo.Level);

                OnLoadMap?.Invoke(map);
            }
            catch(WebException ex)
            {
                var r = ex.Response as HttpWebResponse;

                if (r != null && r.StatusCode == HttpStatusCode.NotFound)
                    MessageUtil.ShowInfo("Level not found");
                else
                    throw;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void NewExecute(object obj)
        {
            try
            {
                var text   = "Are you sure you want to clear this level?" + Environment.NewLine + Environment.NewLine + "All unsaved data will be lost.";
                var result = UserQuestionWindow.Show(text, "Confirm", false);

                if(result != UserQuestionWindow.QuestionResult.Yes)
                    return;

                Mouse.OverrideCursor = Cursors.Wait;

                OnLoadMap?.Invoke(new Map());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void TestExecute(object obj)
        {
        }
    }
}
