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
        public RelayCommand DeselectCommand { get; }
        public RelayCommand ReverseArrowsCommand { get; }


        public ToolCommands Tools { get; }
        private readonly Game _game;



        public MenuCommands(Game game)
        {
            if (game == null)
                throw new Exception("Invalid args to init...");

            Tools = new ToolCommands(game);
            _game = game;

            SelectCommand = Tools.SelectCommand;
            NavigatorCommand = Tools.NavigatorCommand;
            DeselectCommand = Tools.DeselectCommand;

            BuildCommand = MenuCommand(BuildMenu);
            TransformCommand = MenuCommand(TransformMenu);
            DeleteCommand = MenuCommand(DeleteMenu);
            EditCommand = MenuCommand(EditMenu);
            AdvancedCommand = MenuCommand(AdvancedMenu);
            InfoCommand = MenuCommand(InfoMenu);
            ReverseArrowsCommand = MenuCommand(ReverseArrowsMenu);
        }



        public RelayCommand MenuCommand(Action menu)
        {
            return new RelayCommand((_) =>
            {
                if(_game.Mode.UsesSidePanel())
                {
                    _game.CleanUserMode(false, false);
                }

                _game.Mode.UpdateGuiState();
                menu?.Invoke();
                _game.Mode.UpdateGuiState();
            });
        }

        public void BuildMenu()
        {
            var w = new MenuWindow("Build Tools");

            w.AddOption("Build Image", Tools.BuildImageCommand);
            w.AddOption("Build Shape", Tools.BuildShapeCommand);
            w.AddOption("Bucket Flood Fill", Tools.FillCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void EditMenu()
        {
            var w = new MenuWindow("Edit Tools");

            w.AddOption("Move Region", Tools.MoveRegionCommand);
            w.AddOption("Replace Block", Tools.ReplaceCommand);
            w.AddOption("Replace Color", Tools.ReplaceArtColorCommand);
            w.AddOption("Reverse Arrows", ReverseArrowsCommand);
            w.ShowDialog();
            w.Execute();
        }


        public void ReverseArrowsMenu()
        {
            var w = new MenuWindow("Reverse Arrows");

            w.AddOption("Left/Right Arrows", Tools.ReverseHorizontalArrowsCommand);
            w.AddOption("Up/Down Arrows", Tools.ReverseVerticalArrowsCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void AdvancedMenu()
        {
            var w = new MenuWindow("Advanced Tools");

            w.AddOption("Connect Teleports", Tools.ConnectTeleportsCommand);
            w.AddOption("Measure Distance", Tools.DistanceCommand);
            w.AddOption("Move Pasted Blocks", Tools.MovePastedBlocksCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void InfoMenu()
        {
            var w = new MenuWindow("Info Tools");

            w.AddOption("Block Count", Tools.BlockCountCommand);
            w.AddOption("Block Info", Tools.BlockInfoCommand);
            w.AddOption("Editor Info", Tools.EditorInfoCommand);
            w.AddOption("Map Info", Tools.MapInfoCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void TransformMenu()
        {
            var w = new MenuWindow("Transform Tools");

            w.AddOption("Rotate", Tools.RotateCommand);
            w.AddOption("Horizontal Flip", Tools.HorizontalFlipCommand);
            w.AddOption("Vertical Flip", Tools.VerticalFlipCommand);

            w.ShowDialog();
            w.Execute();
        }

        public void DeleteMenu()
        {
            if (MySettings.FirstTimeLoad)
            {
                var hint1 = "Hint 1:  You can delete a block by right-clicking it.";
                var hint2 = "Hint 2:  You can deselect anything with the Escape key.";
                MessageUtil.ShowInfo(hint1 + Environment.NewLine + Environment.NewLine + hint2);
                MySettings.FirstTimeLoad = false;
            }

            var w = new MenuWindow("Remove Tools");

            w.AddOption("Remove Block Type", Tools.DeleteBlockTypeCommand);
            w.AddOption("Remove Block Option", Tools.DeleteBlockOptionCommand);
            w.AddOption("Eraser", Tools.DeleteModeCommand);
            w.AddOption("Remove Region", Tools.DeleteRegionCommand);


            w.ShowDialog();
            w.Execute();
        }

    }
}
