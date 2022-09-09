using BlockEditor.Helpers;
using BlockEditor.Views.Windows;
using System;

namespace BlockEditor.Models
{
    public class MenuCommands
    {

        public RelayCommand BuildCommand { get; }
        public RelayCommand TransformCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand InfoCommand { get; }
        public RelayCommand AdvancedCommand { get; }
        public RelayCommand SelectCommand { get; }
        public RelayCommand NavigatorCommand { get; }
        public RelayCommand NavigateCommand { get; }
        public RelayCommand DeselectCommand { get; }


        private readonly ToolCommands _tools;
        private readonly Game _game;



        public MenuCommands(Game game)
        {
            if (game == null)
                throw new Exception("Invalid args to init...");

            _tools = new ToolCommands(game);
            _game = game;

            SelectCommand = _tools.SelectCommand;
            NavigatorCommand = _tools.NavigatorCommand;
            NavigateCommand = _tools.NavigateCommand;
            DeselectCommand = _tools.DeselectCommand;

            BuildCommand = MenuCommand(BuildMenu);
            TransformCommand = MenuCommand(TransformMenu);
            DeleteCommand = MenuCommand(DeleteMenu);
            EditCommand = MenuCommand(EditMenu);
            AdvancedCommand = MenuCommand(AdvancedMenu);
            InfoCommand = MenuCommand(InfoMenu);
        }



        public RelayCommand MenuCommand(Action menu)
        {
            return new RelayCommand((_) =>
            {
                _game.Mode.UpdateGuiState();
                menu?.Invoke();
                _game.Mode.UpdateGuiState();
            });
        }

        public void BuildMenu()
        {
            var w = new MenuWindow("Build Tools");

            w.AddOption("Add Shape", _tools.AddShapeCommand);
            w.AddOption("Add Image", _tools.AddImageCommand);
            w.AddOption("Bucket Flood Fill", _tools.FillCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void EditMenu()
        {
            var w = new MenuWindow("Edit Tools");

            w.AddOption("Move Region", _tools.MoveRegionCommand);
            w.AddOption("Replace Block", _tools.ReplaceCommand);
            w.AddOption("Replace Color", _tools.ReplaceArtColorCommand);
            w.AddOption("Reverse Traps", _tools.ReverseTrapsCommand);
            w.ShowDialog();
            w.Execute();
        }

        public void AdvancedMenu()
        {
            var w = new MenuWindow("Advanced Tools");

            w.AddOption("Connect Teleports", _tools.ConnectTeleportsCommand);
            w.AddOption("Measure Distance", _tools.DistanceCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void InfoMenu()
        {
            var w = new MenuWindow("Info Tools");

            w.AddOption("Block Count", _tools.BlockCountCommand);
            w.AddOption("Block Info", _tools.BlockInfoCommand);
            w.AddOption("Editor Info", _tools.EditorInfoCommand);
            w.AddOption("Map Info", _tools.MapInfoCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void TransformMenu()
        {
            var w = new MenuWindow("Transform Tools");

            w.AddOption("Rotate", _tools.RotateCommand);
            w.AddOption("Horizontal Flip", _tools.HorizontalFlipCommand);
            w.AddOption("Vertical Flip", _tools.VerticalFlipCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void DeleteMenu()
        {
            var w = new MenuWindow("Delete Tools");

            w.AddOption("Block Type", _tools.DeleteBlockTypeCommand);
            w.AddOption("Block Option", _tools.DeleteBlockOptionCommand);
            w.AddOption("Delete Blocks", _tools.DeleteCommand);
            w.AddOption("Delete Region", _tools.DeleteRegionCommand);

            w.ShowDialog();
            w.Execute();
        }

    }
}
