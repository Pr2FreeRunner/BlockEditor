using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views;
using DataAccess;
using Parsers;

namespace BlockEditor.ViewModels
{
    public class MapButtonsViewModel : NotificationObject
    {

        public event Action<Blocks> OnLoadMap;


        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand NewCommand { get; set; }
        public RelayCommand TestCommand { get; set; }




        public MapButtonsViewModel()
        {
            LoadCommand = new RelayCommand(LoadExecute);
            SaveCommand = new RelayCommand(SaveExecute);
            NewCommand  = new RelayCommand(NewExecute);
            TestCommand = new RelayCommand(TestExecute);
        }

        private void SaveExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void LoadExecute(object obj)
        {
            var input = UserInputWindow.Show("Level ID: ", "Input");

            if(string.IsNullOrWhiteSpace(input))
                return;

            if(!Converters.TryParse(input, out var id))
            {
                MessageUtil.ShowError("Invalid Level ID");
                return;
            }

            var data = new PR2Accessor().Download(id);

            if (string.IsNullOrWhiteSpace(data))
            {
                MessageUtil.ShowError("Failed to download level");
                return;
            }

            var levelInfo = new PR2Parser().ParseLevel(data);


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

            var blocks = Converters.ToBlocks(levelInfo.Level);

            OnLoadMap?.Invoke(blocks);
        }

        private void NewExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void TestExecute(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
