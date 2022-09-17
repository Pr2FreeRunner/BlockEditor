using System;
using System.Linq;
using System.Net;
using System.Windows.Input;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Windows;
using DataAccess;
using LevelModel.Models;
using Parsers;

namespace BlockEditor.ViewModels
{
    public class MapButtonsViewModel : NotificationObject
    {

        public event Action<Map> OnLoadMap;
        public event Action OnSaveMap;
        public event Action OnTestMap;


        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand NewCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand AccountCommand { get; set; }


        public MapButtonsViewModel()
        {
            LoadCommand = new RelayCommand(LoadExecute);
            SaveCommand = new RelayCommand(SaveExecute);
            NewCommand = new RelayCommand(NewExecute);
            TestCommand = new RelayCommand(TestExecute);
            AccountCommand = new RelayCommand(LoginExecute);

        }

        private void ResetUserMode()
        {
            App.MyMainWindow?.ResetUserMode();
        }

        private void LoginExecute(object obj)
        {
            ResetUserMode();
            new AccountWindow().ShowDialog();
        }

        private void SaveExecute(object obj)
        {
            ResetUserMode();

            OnSaveMap?.Invoke();
        }

        private void LoadLevel(Level level)
        {
            if(level == null)
                return;

            var map = new Map(level);

            OnLoadMap?.Invoke(map);
        }

        private void LoadLevel(int id, bool unpublished)
        {
            using(new TempCursor(Cursors.Wait))
            {
                try
                {
                    var data = PR2Accessor.Download(id);

                    if (string.IsNullOrWhiteSpace(data))
                    {
                        MessageUtil.ShowError("Failed to download level.");
                        return;
                    }

                    var levelInfo = PR2Parser.Level(data);

                    if (levelInfo == null)
                    {
                        MessageUtil.ShowError("Failed to parse level.");
                        return;
                    }

                    if (levelInfo.Messages == null || levelInfo.Messages.Any())
                    {
                        MessageUtil.ShowMessages(levelInfo.Messages);
                        return;
                    }

                    if(unpublished)
                        levelInfo.Level.Published = false;

                    LoadLevel(levelInfo.Level);
                }
                catch (WebException ex)
                {
                    var r = ex.Response as HttpWebResponse;

                    if (r != null && r.StatusCode == HttpStatusCode.NotFound)
                        MessageUtil.ShowInfo("Level not found.");
                    else
                        throw;
                }
            }
        }

        private void LoadExecute(object obj)
        {
            ResetUserMode();

            var window  = new LoadMapWindow();
            var success = window.ShowDialog();

            if(!success.HasValue || !success.Value)
                return;

            if(window.SelectedLevel == null)
                LoadLevel(window.SelectedLevelID, window.Unpublish);
            else
            {
                if(window.Unpublish)
                    window.SelectedLevel.Published = false;

                LoadLevel(window.SelectedLevel);
            }
        }

        private void NewExecute(object obj)
        {
            ResetUserMode();

            try
            {
                var text   = "Are you sure you want to clear this level?" + Environment.NewLine + Environment.NewLine + "All unsaved data will be lost.";
                var result = UserQuestionWindow.Show(text, "New Level", false);

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
            ResetUserMode();

            OnTestMap?.Invoke();
        }
    }
}
