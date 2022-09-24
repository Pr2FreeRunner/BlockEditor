using BlockEditor.Utils;
using BlockEditor.Helpers;
using BlockEditor.Views.Controls;
using BlockEditor.Views.Windows;
using Builders.DataStructures.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LevelModel.Models.Components;

using static BlockEditor.Models.UserMode;

namespace BlockEditor.Models
{
    public class ToolCommands
    {

        public RelayCommand NavigatorCommand { get; }
        public RelayCommand NavigateCommand { get; }
        public RelayCommand FillCommand { get; }
        public RelayCommand SelectCommand { get; }
        public RelayCommand BuildShapeCommand { get; }
        public RelayCommand BlockInfoCommand { get; }
        public RelayCommand MapInfoCommand { get; }
        public RelayCommand BlockCountCommand { get; }
        public RelayCommand ReplaceCommand { get; }
        public RelayCommand BuildImageCommand { get; }
        public RelayCommand RotateCommand { get; }
        public RelayCommand VerticalFlipCommand { get; }
        public RelayCommand HorizontalFlipCommand { get; }
        public RelayCommand DeleteBlockTypeCommand { get; }
        public RelayCommand EditorInfoCommand { get; }
        public RelayCommand ConnectTeleportsCommand { get; }
        public RelayCommand MoveRegionCommand { get; }
        public RelayCommand ReplaceArtColorCommand { get; }
        public RelayCommand DeleteRegionCommand { get; }
        public RelayCommand DeleteBlockOptionCommand { get; }
        public RelayCommand ReverseHorizontalArrowsCommand { get; }
        public RelayCommand ReverseVerticalArrowsCommand { get; }
        public RelayCommand DistanceCommand { get; }
        public RelayCommand DeleteModeCommand { get; }
        public RelayCommand DeselectCommand { get; }
        public RelayCommand MovePastedBlocksCommand { get; }

        public ToolCommands(Game game)
        {
            FillCommand = new RelayCommand((_) => FloodFill(game));
            SelectCommand = new RelayCommand((_) => Selection(game));
            BuildShapeCommand = new RelayCommand((_) => BuildShape(game), (_) => HasUserSelectedRegion(game));
            BlockInfoCommand = new RelayCommand((_) => BlockInfo(game));
            MapInfoCommand = new RelayCommand((_) => MapInfo(game));
            BlockCountCommand = new RelayCommand((_) => BlockCount(game));
            ReplaceCommand = new RelayCommand((_) => Replace(game));
            BuildImageCommand = new RelayCommand((_) => BuildImage(game));
            RotateCommand = new RelayCommand((_) => Rotate(game));
            VerticalFlipCommand = new RelayCommand((_) => VerticalFlip(game));
            HorizontalFlipCommand = new RelayCommand((_) => HorizontalFlip(game));
            DeleteBlockTypeCommand = new RelayCommand((_) => DeleteBlockType(game));
            EditorInfoCommand = new RelayCommand((_) => EditorInfo(game));
            ConnectTeleportsCommand = new RelayCommand((_) => ConnectTeleports(game));
            MoveRegionCommand = new RelayCommand((_) => MoveRegion(game));
            DeleteRegionCommand = new RelayCommand((_) => DeleteRegion(game));
            DistanceCommand = new RelayCommand((_) => Distance(game));
            DeleteModeCommand = new RelayCommand((_) => DeleteMode(game));
            DeselectCommand = new RelayCommand((_) => Deselect(game), (_) => DeselectCanExecute(game));
            NavigatorCommand = new RelayCommand((_) => Navigator(game, SimpleBlock.None));
            NavigateCommand = new RelayCommand((p) => Navigator(game, p));
            DeleteBlockOptionCommand = new RelayCommand((_) => DeleteBlockOption(game));
            ReverseHorizontalArrowsCommand = new RelayCommand((_) => ReverseHorizontalArrows(game));
            ReplaceArtColorCommand = new RelayCommand((_) => ReplaceArtColor(game));
            ReverseVerticalArrowsCommand = new RelayCommand((_) => ReverseVerticalArrows(game));
            MovePastedBlocksCommand = new RelayCommand((_) => MovePastedBlocks(game), (_) => MovePastedBlocksCanExecute(game));
        }

        
        private void MovePastedBlocks(Game game)
        {
            game.CleanUserMode(true, true);

            if(game.UserOperations.LastAddBlocksOperation == null)
                return;

            var blocksToRemove = game.UserOperations.LastAddBlocksOperation.GetBlocks().ToList();

            var w = new MovePastedBlocksWindow(blocksToRemove);

            w.ShowDialog();

            if(w.DialogResult != true)
                return;

            using(new TempCursor(Cursors.Wait))
            {
                BlocksUtil.Move(game, blocksToRemove, w.MoveX.Value, w.MoveY.Value);
            }
        }

        private bool MovePastedBlocksCanExecute(Game game)
        {
            return game.UserOperations.LastAddBlocksOperation != null;
        }

        private bool HasUserSelectedRegion(Game game)
        {
            return game.UserSelection.HasSelectedRegion;
        }

        private void ReverseHorizontalArrows(Game game)
        {
            using (new TempOverwrite(game.Map.Blocks, true))
            using (new TempCursor(Cursors.Wait))
            {
                var remove = new List<int> { Block.ARROW_LEFT, Block.ARROW_RIGHT };
                var add = new List<int> { Block.ARROW_RIGHT, Block.ARROW_LEFT };
                var blocks = BlocksUtil.ReplaceBlock(game.Map?.Blocks, remove, add, game.UserSelection.MapRegion);

                game.AddBlocks(blocks);
            }
        }

        private void ReverseVerticalArrows(Game game)
        {
            using (new TempOverwrite(game.Map.Blocks, true))
            using (new TempCursor(Cursors.Wait))
            {
                var remove = new List<int> { Block.ARROW_UP, Block.ARROW_DOWN };
                var add = new List<int> { Block.ARROW_DOWN, Block.ARROW_UP };
                var blocks = BlocksUtil.ReplaceBlock(game.Map?.Blocks, remove, add, game.UserSelection.MapRegion);

                game.AddBlocks(blocks);
            }
        }

        private bool DeselectCanExecute(Game game)
        {
            if (BlockSelection.SelectedBlocks != null)
                return true;

            if (BlockSelection.SelectedBlock != null)
                return true;

            if (BlockSelection.SelectedBlockOption != null)
                return true;

            if (game.Mode.Value != UserModes.None)
                return true;

            if (Mouse.OverrideCursor != null)
                return true;

            if (game.UserSelection.HasSelectedRegion)
                return true;

            if (game.MeasureDistance.MapPoint1 != null)
                return true;

            return false;
        }

        private void Deselect(Game game)
        {
            game.CleanUserMode(true, true);
        }

        private void DeleteMode(Game game)
        {
            if (game.Mode.Value != UserModes.Delete)
            {
                game.CleanUserMode(true, false);
                game.Mode.Value = UserModes.Delete;
            }
            else
            {
                game.CleanUserMode(true, false);
            };
        }

        private void Distance(Game game)
        {
            if (game.Mode.Value != UserModes.Distance)
            {
                game.CleanUserMode(true, false);
                game.Mode.Value = UserModes.Distance;
            }
            else
            {
                game.CleanUserMode(true, false);
            };
        }

        private void Navigator(Game game, object o)
        {
            if (o is SimpleBlock block)
                Navigator(game, block);
            else
                Navigator(game, SimpleBlock.None);
        }

        private void Navigator(Game game, SimpleBlock block)
        {
            game.CleanUserMode(false, true);
            int? id = null;
            Predicate<SimpleBlock> filter = null;

            if (block.IsEmpty())
            {
                id = SelectBlockWindow.Show("Navigator", true);

                if (id == null)
                    return;

                filter = (b) => b.ID == id.Value;
            }
            else
            {
                id = block.ID;
                filter = (b) => b.ID == id.Value && string.Equals(b.Options, block.Options, StringComparison.InvariantCultureIgnoreCase);
            }

            if (Block.IsStartBlock(id.Value))
                game.GoToStartPosition(id.Value);
            else
                NavigatorWindow.Show(game, filter, id.Value, block.Position);
        }

        private void Selection(Game game)
        {
            if (game.Mode.Value != UserModes.Selection)
            {
                game.CleanUserMode(true, true);
                game.Mode.Value = UserModes.Selection;
            }
            else
            {
                game.CleanUserMode(false, false);
            }
        }

        private void ConnectTeleports(Game game)
        {
            if (game.Mode.Value != UserModes.ConnectTeleports)
            {
                game.CleanUserMode(true, true);
                game.Mode.Value = UserModes.ConnectTeleports;
                App.MyMainWindow?.CurrentMap?.AddSidePanel(new ConnectTeleportsControl(game));
            }
            else
            {
                game.CleanUserMode(true, true);
            }
        }

        private void FloodFill(Game game)
        {
            if (game.Mode.Value != UserModes.Fill)
            {
                game.CleanUserMode(true, false);
                game.Mode.Value = UserModes.Fill;
            }
            else
            {
                game.Mode.Value = UserModes.None;
            }
        }

        private void EditorInfo(Game game)
        {
            game.CleanUserMode(true, true);

            new EditorInfoWindow().ShowDialog();

            game.Mode.Value = UserModes.None;
        }

        private void VerticalFlip(Game game)
        {
            if (BlockSelection.VerticalFlipCommand.CanExecute(null))
            {
                BlockSelection.VerticalFlipCommand.Execute(null);
            }
            else if (game.UserSelection.HasSelectedRegion)
            {
                game.VerticalFlipRegion(game.UserSelection.MapRegion);
                ReverseVerticalArrows(game);
            }
            else
            {
                var r1 = UserQuestionWindow.Show("Do you wish to vertically flip the map?"
                    + Environment.NewLine + Environment.NewLine
                    + "Note:  Art will not be moved.", 
                    "Vertical Flip", false);

                if (r1 != UserQuestionWindow.QuestionResult.Yes)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    game.Engine.PauseConfirmed();
                    game.VerticalFlipMap();
                    game.GoToStartPosition(showError: false);
                    game.Engine.Pause = false;
                }
            }
        }
        
        private void HorizontalFlip(Game game)
        {
            if (BlockSelection.HorizontalFlipCommand.CanExecute(null))
            {
                BlockSelection.HorizontalFlipCommand.Execute(null);
            }
            else if (game.UserSelection.HasSelectedRegion)
            {
                game.HorizontalFlipRegion(game.UserSelection.MapRegion);
                ReverseHorizontalArrows(game);
            }
            else
            {
                var r = UserQuestionWindow.Show("Do you wish to horizontally flip the map?"
                      + Environment.NewLine + Environment.NewLine
                      + "Note:  Art will not be moved. ", 
                      "Horizontal Flip", false);

                if (r != UserQuestionWindow.QuestionResult.Yes)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    game.Engine.PauseConfirmed();
                    game.HorizontalFlipMap();
                    ReverseHorizontalArrows(game);
                    game.GoToStartPosition(showError: false);
                    game.Engine.Pause = false;
                }
            }
        }
       
        private void Rotate(Game game)
        {
            if (BlockSelection.RotateCommand.CanExecute(null))
            {
                BlockSelection.RotateCommand.Execute(null);
            }
            else if (game.UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (game.UserSelection.SelectedRegionContainsBlocks(game.Map))
                    text = "If you wish to rotate the blocks inside the selected region, "
                        + Environment.NewLine + "you first have to select them by using Ctrl + C or Ctrl + X";
                else
                    text = "The selected region contains no blocks, there is nothing to rotate.";

                MessageUtil.ShowInfo(text);
            }
            else
            {
                var r = UserQuestionWindow.Show("Do you wish to rotate the map?"
                      + Environment.NewLine + Environment.NewLine
                      + "Note:  Art will not be moved. ", 
                      "Rotate Blocks", false);

                if (r != UserQuestionWindow.QuestionResult.Yes)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    game.Engine.PauseConfirmed();
                    game.RotateMap();
                    game.GoToStartPosition(showError: false);
                    game.Engine.Pause = false;
                }
            }
        }
        
        private void MoveRegion(Game game)
        {
            game.CleanUserMode(true, false);

            var w = new EditRegionWindow(game.Map, game.UserSelection.MapRegion, EditRegionWindow.EditArtModes.Move);

            w.ShowDialog();

            using (new TempCursor(Cursors.Wait))
            {
                if (w.BlocksToRemove.AnyBlocks())
                    game.RemoveBlocks(w.BlocksToRemove);

                if (w.BlocksToAdd.AnyBlocks())
                    game.AddBlocks(w.BlocksToAdd);
            }
        }

        private void DeleteRegion(Game game)
        {
            game.CleanUserMode(true, false);

            var w = new EditRegionWindow(game.Map, game.UserSelection.MapRegion, EditRegionWindow.EditArtModes.Delete);

            w.ShowDialog();

            using (new TempCursor(Cursors.Wait))
            {
                if (w.BlocksToRemove != null && w.BlocksToRemove.Any())
                    game.RemoveBlocks(w.BlocksToRemove);

                if (w.BlocksToAdd != null && w.BlocksToAdd.Any())
                    game.AddBlocks(w.BlocksToAdd);
            }
        }

        private void ReplaceArtColor(Game game)
        {
            game.CleanUserMode(true, false);

            var w = new EditRegionWindow(game.Map, game.UserSelection.MapRegion, EditRegionWindow.EditArtModes.ReplaceColor);

            w.ShowDialog();
        }

        private void BuildShape(Game game)
        {
            game.CleanUserMode(true, false);

            var selectedId = SelectBlockWindow.Show("Build Shape", false);
            var region = game.UserSelection.MapRegion;

            if (selectedId == null)
                return;

            if (!ShapeBuilderUtil.PickShape())
                return;

            var blocks = ShapeBuilderUtil.Build(game.Map, selectedId.Value, region);

            if (!blocks.AnyBlocks() && region.IsComplete() && !game.Map.Blocks.Overwrite && ShapeBuilderUtil.Probablity != 100)
                throw new OverwriteException();

            game.AddBlocks(blocks);
        }

        private void BuildImage(Game game, MyPoint? p = null)
        {
            game.CleanUserMode(true, true);

            var w = new BuildImageWindow(p);
            var r = w.ShowDialog();

            if (w.GetPosition)
            {
                game.Mode.Value = UserModes.GetPosition;
                game.GetPosition = (x) => BuildImage(game, x);
                return;
            }

            if (r != true || w.BuildInfo == null)
                return;


            using (new TempCursor(Cursors.Wait))
            {
                var level = Builders.PR2Builder.BuildLevel(w.BuildInfo);

                if (level == null)
                    throw new Exception("Something went wrong....");

                if (w.BuildInfo.ImageInfo.Type == ImageDTO.ImageType.Blocks)
                {
                    var pr2Blocks = level.Blocks.Skip(8).ToList();
                    w.ShiftPosition(pr2Blocks);
                    var blocks = BlocksUtil.ToBlocks(pr2Blocks, out var blocksOutsideBoundries).GetBlocks();
                    var position = blocks.First().Position;

                    MyUtil.BlocksOutsideBoundries(blocksOutsideBoundries);
                    game.AddBlocks(blocks);
                    game.GoToPosition(position);
                }
                else
                {
                    w.ShiftPosition(level.DrawArt0);
                    w.ShiftPosition(level.DrawArt1);

                    game.Map.Level.DrawArt1.AddRange(level.DrawArt1);
                    game.Map.Level.DrawArt0.AddRange(level.DrawArt0);
                    game.Map.LoadArt();
                }
            }
        }

        private void DeleteBlockType(Game game)
        {
            game.CleanUserMode(true, false);

            var region = game.UserSelection.MapRegion;
            var id1 = SelectBlockWindow.Show("Block Type to Remove:", false);

            if (id1 == null)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                var blocks = BlocksUtil.RemoveBlocks(game.Map?.Blocks, new List<int>() { id1.Value }, region);

                game.RemoveBlocks(blocks);
            }
        }

        private void DeleteBlockOption(Game game)
        {
            game.CleanUserMode(true, false);

            var region = game.UserSelection.MapRegion;
            var remove = new List<SimpleBlock>();
            var add = new List<SimpleBlock>();

            using (new TempCursor(Cursors.Wait))
            {
                foreach (var oldBlock in BlocksUtil.GetBlocks(game.Map?.Blocks, region))
                {
                    if (oldBlock.IsEmpty())
                        continue;

                    var newBlock = new SimpleBlock(oldBlock.ID, oldBlock.Position.Value, string.Empty);
                    remove.Add(oldBlock);
                    add.Add(newBlock);
                }


                game.RemoveBlocks(remove);
                game.AddBlocks(add);
            }
        }

        private void Replace(Game game)
        {
            game.CleanUserMode(true, false);


            using (new TempOverwrite(game.Map.Blocks, true))
            {
                var region = game.UserSelection.MapRegion;
                var id1 = SelectBlockWindow.Show("Block to Replace:", false);

                if (id1 == null)
                    return;

                var id2 = SelectBlockWindow.Show("Block to Add:", false);
                if (id2 == null)
                    return;

                var input = UserInputWindow.Show("Replace Probability: ", "Replace", "100");

                if(!MyUtil.TryParse(input, out var probability))
                {
                    if(!string.IsNullOrEmpty(input))
                        throw new Exception("Invalid input");
                    
                    return;
                }

                using (new TempCursor(Cursors.Wait))
                {
                    var blocks = BlocksUtil.ReplaceBlock(game.Map?.Blocks, new List<int>() { id1.Value }, new List<int>() { id2.Value }, region);

                    game.AddBlocks(blocks.Where(b => RandomUtil.GetRandom(1, 99) < probability));
                }
            }
        }

        private void BlockInfo(Game game)
        {
            if (game.Mode.Value != UserModes.BlockInfo)
            {
                if(MySettings.FirstBlockInfo)
                {
                    MessageUtil.ShowInfo("Hint: To directly open the Block-Info window," 
                        + Environment.NewLine +"you can hold down Ctrl while clicking a block.");
                    MySettings.FirstBlockInfo = false;
                }
                game.CleanUserMode(true, true);
                game.Mode.Value = UserModes.BlockInfo;
            }
            else
            {
                game.Mode.Value = UserModes.None;
            }
        }

        private void BlockCount(Game game)
        {
            game.CleanUserMode(true, true);

            var w = new BlockCountWindow(game.Map);
            w.ShowDialog();
        }

        private void MapInfo(Game game)
        {
            game.CleanUserMode(true, true);

            var w = new MapInfoWindow(game.Map, game.Engine.RefreshGui);
            w.ShowDialog();
        }

        internal void StartBlockInfo(Game game, MyPoint? index)
        {
            if(index == null)
                return;

            bool navigate = false;
            using (new TempCursor(null))
            {
                var w = new BlockOptionWindow(game.Map, index, game.Engine.RefreshGui);
                w.ShowDialog();

                navigate = w.StartNavigation;
            }

            if (navigate)
            {
                game.Mode.Value = UserModes.None;

                if (NavigateCommand.CanExecute(null))
                    NavigateCommand.Execute(game.Map.Blocks.GetBlock(index));
            }
        }

        internal void StartFloodFill(Game game, MyPoint? index)
        {
            if(index == null)
                return;

            var selectedId = SelectBlockWindow.Show("Flood Fill", false);

            if (selectedId == null)
                return;

            if (Block.IsStartBlock(selectedId))
                throw new Exception("Flood fill with start block is not allowed.");

            using (new TempCursor(Cursors.Wait))
            {
                var b = game.Map.Blocks.GetBlock(index);

                if (!b.IsEmpty() && !game.Map.Blocks.Overwrite)
                    throw new OverwriteException();

                var region = game.UserSelection.HasSelectedRegion ? game.UserSelection.MapRegion : null;
                game.AddBlocks(BlocksUtil.GetFloodFill(game.Map?.Blocks, index, selectedId.Value, region));
            }
        }
    }
}
