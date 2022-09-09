﻿using BlockEditor.Helpers;
using BlockEditor.Utils;
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
        public RelayCommand AddShapeCommand { get; }
        public RelayCommand BlockInfoCommand { get; }
        public RelayCommand MapInfoCommand { get; }
        public RelayCommand BlockCountCommand { get; }
        public RelayCommand ReplaceCommand { get; }
        public RelayCommand AddImageCommand { get; }
        public RelayCommand RotateCommand { get; }
        public RelayCommand VerticalFlipCommand { get; }
        public RelayCommand HorizontalFlipCommand { get; }
        public RelayCommand DeleteBlockTypeCommand { get; }
        public RelayCommand SettingsCommand { get; }
        public RelayCommand ConnectTeleportsCommand { get; }
        public RelayCommand MoveRegionCommand { get; }
        public RelayCommand ReplaceArtColorCommand { get; }
        public RelayCommand DeleteRegionCommand { get; }
        public RelayCommand DeleteBlockOptionCommand { get; }
        public RelayCommand ReverseHorizontalArrowsCommand { get; }
        public RelayCommand ReverseVerticalArrowsCommand { get; }
        public RelayCommand DistanceCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand DeselectCommand { get; }


        public ToolCommands(Game game)
        {
            FillCommand = new RelayCommand((_) => FloodFill(game));
            SelectCommand = new RelayCommand((_) => Selection(game));
            AddShapeCommand = new RelayCommand((_) => AddShape(game), (_) => game.UserSelection.HasSelectedRegion);
            BlockInfoCommand = new RelayCommand((_) => BlockInfo(game));
            MapInfoCommand = new RelayCommand((_) => MapInfo(game));
            BlockCountCommand = new RelayCommand((_) => BlockCount(game));
            ReplaceCommand = new RelayCommand((_) => Replace(game));
            AddImageCommand = new RelayCommand((_) => AddImage(game));
            RotateCommand = new RelayCommand((_) => Rotate(game));
            VerticalFlipCommand = new RelayCommand((_) => VerticalFlip(game));
            HorizontalFlipCommand = new RelayCommand((_) => HorizontalFlip(game));
            DeleteBlockTypeCommand = new RelayCommand((_) => DeleteBlockType(game));
            SettingsCommand = new RelayCommand((_) => EditorSettings(game));
            ConnectTeleportsCommand = new RelayCommand((_) => ConnectTeleports(game));
            MoveRegionCommand = new RelayCommand((_) => MoveRegion(game));
            DeleteRegionCommand = new RelayCommand((_) => DeleteRegion(game));
            DistanceCommand = new RelayCommand((_) => Distance(game));
            DeleteCommand = new RelayCommand((_) => Delete(game));
            DeselectCommand = new RelayCommand((_) => Deselect(game), (_) => DeselectCanExecute(game));
            NavigatorCommand = new RelayCommand((_) => Navigator(game, SimpleBlock.None));
            NavigateCommand = new RelayCommand((p) => Navigator(game, p));
            DeleteBlockOptionCommand = new RelayCommand((_) => DeleteBlockOption(game));
            ReverseHorizontalArrowsCommand = new RelayCommand((_) => ReverseHorizontalArrows(game));
            ReverseVerticalArrowsCommand = new RelayCommand((_) => ReverseVerticalArrows(game));
            ReplaceArtColorCommand = new RelayCommand((_) => ReplaceArtColor(game));
        }


        private void ReverseHorizontalArrows(Game game)
        {
            game.CleanUserMode(true, false);

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
            game.CleanUserMode(true, false);

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

            if (game.UserSelection.MapRegion.Point1 != null)
                return true;

            if (game.MeasureDistance.MapPoint1 != null)
                return true;

            return false;
        }

        private void Deselect(Game game)
        {
            game.CleanUserMode(true, true);
        }

        private void Delete(Game game)
        {
            BlockSelection.Reset();
            game.MeasureDistance.Reset();

            if (game.Mode.Value != UserModes.Delete)
            {
                game.Mode.Value = UserModes.Delete;
            }
            else
            {
                game.Mode.Value = UserModes.None;
            };
        }

        private void Distance(Game game)
        {
            BlockSelection.Reset();
            game.MeasureDistance.Reset();

            if (game.Mode.Value != UserModes.Distance)
            {
                game.Mode.Value = UserModes.Distance;
            }
            else
            {
                game.Mode.Value = UserModes.None;
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
            BlockSelection.Reset();
            game.UserSelection.Reset();
            game.MeasureDistance.Reset();

            if (game.Mode.Value != UserModes.Selection)
            {
                game.Mode.Value = UserModes.Selection;
            }
            else
            {
                game.Mode.Value = UserModes.None;
            }
        }

        private void ConnectTeleports(Game game)
        {
            BlockSelection.Reset();
            game.UserSelection.Reset();
            game.MeasureDistance.Reset();

            if (game.Mode.Value != UserModes.ConnectTeleports)
            {
                Models.ConnectTeleports.Start();
                game.Mode.Value = UserModes.ConnectTeleports;
            }
            else
            {
                var blocks = Models.ConnectTeleports.End(out var addMore);

                if (addMore)
                {
                    game.Mode.Value = UserModes.ConnectTeleports; // updates GUI
                    return;
                }

                using (new TempOverwrite(game.Map.Blocks, true))
                {
                    game.AddBlocks(blocks);
                    game.Mode.Value = UserModes.None;
                }
            }
        }

        private void FloodFill(Game game)
        {
            if (game.Mode.Value != UserModes.Fill)
            {
                BlockSelection.Reset();
                game.MeasureDistance.Reset();
                game.Mode.Value = UserModes.Fill;
            }
            else
            {
                game.Mode.Value = UserModes.None;
            }
        }

        private void EditorSettings(Game game)
        {
            game.CleanUserMode(true, true);

            new SettingsWindow().ShowDialog();

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
                string text = "";

                if (game.UserSelection.SelectedRegionContainsBlocks(game.Map))
                    text = "If you wish to flip the blocks inside the selected region, "
                        + Environment.NewLine + "you first have to select them by using Ctrl + C or Ctrl + X";
                else
                    text = "The selected region contains no blocks, there is nothing to flip.";

                MessageUtil.ShowInfo(text);
            }
            else
            {
                var r1 = UserQuestionWindow.Show("Do you wish vertically flip the map?", "Vertical Flip", false);

                if (r1 != UserQuestionWindow.QuestionResult.Yes)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    game.Engine.PauseConfirmed();
                    game.Map.Blocks.VerticalFlip();
                    game.GoToStartPosition();
                    game.Engine.Pause = false;
                }
            }
        }

        private void MoveRegion(Game game)
        {
            game.CleanUserMode(true, false);

            var w = new EditArtWindow(game.Map, game.UserSelection.MapRegion, EditArtWindow.EditArtModes.Move);

            w.ShowDialog();

            using (new TempCursor(Cursors.Wait))
            {
                if (w.BlocksToRemove != null && w.BlocksToRemove.Any())
                    game.RemoveBlocks(w.BlocksToRemove);

                if (w.BlocksToAdd != null && w.BlocksToAdd.Any())
                    game.AddBlocks(w.BlocksToAdd);
            }
        }

        private void DeleteRegion(Game game)
        {
            game.CleanUserMode(true, false);

            var w = new EditArtWindow(game.Map, game.UserSelection.MapRegion, EditArtWindow.EditArtModes.Delete);

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

            var w = new EditArtWindow(game.Map, game.UserSelection.MapRegion, EditArtWindow.EditArtModes.ReplaceColor);

            w.ShowDialog();
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
                var r = UserQuestionWindow.Show("Do you wish rotate the map?", "Rotate Blocks", false);

                if (r != UserQuestionWindow.QuestionResult.Yes)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    game.Engine.PauseConfirmed();
                    game.Map.Blocks.Rotate();
                    game.GoToStartPosition();
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
                string text = "";

                if (game.UserSelection.SelectedRegionContainsBlocks(game.Map))
                    text = "If you wish to flip the blocks inside the selected region, "
                        + Environment.NewLine + "you first have to select them by using Ctrl + C or Ctrl + X";
                else
                    text = "The selected region contains no blocks, there is nothing to flip.";

                MessageUtil.ShowInfo(text);
            }
            else
            {
                var r = UserQuestionWindow.Show("Do you wish horizontally flip the map?", "Horizontal Flip", false);

                if (r != UserQuestionWindow.QuestionResult.Yes)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    game.Engine.PauseConfirmed();
                    game.Map.Blocks.HorizontalFlip();
                    game.GoToStartPosition();
                    game.Engine.Pause = false;
                }
            }
        }

        private void AddShape(Game game)
        {
            game.CleanUserMode(true, false);

            var selectedId = SelectBlockWindow.Show("Add Shape", false);
            var region = game.UserSelection.MapRegion;

            if (selectedId == null)
                return;

            if (!ShapeBuilderUtil.PickShape())
                return;

            var blocks = ShapeBuilderUtil.Build(game.Map, selectedId.Value, region);

            if (blocks != null && !blocks.Any() && region != null && region.IsComplete() && !game.Map.Blocks.Overwrite && ShapeBuilderUtil.Probablity != 100)
                throw new OverwriteException();

            game.AddBlocks(blocks);
            BlockSelection.Reset();
        }

        private void AddImage(Game game, MyPoint? p = null)
        {
            game.CleanUserMode(true, true);

            var w = new ImageToBlocksWindow(p);
            var r = w.ShowDialog();

            if (w.GetPosition)
            {
                game.Mode.Value = UserModes.GetPosition;
                game.GetPosition = (x) => AddImage(game, x);
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

                    MessageUtil.ShowInfo("The image has been added to the map." + Environment.NewLine + Environment.NewLine + "Note:  Art is not visible inside the Block Editor.");
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

                using (new TempCursor(Cursors.Wait))
                {
                    var blocks = BlocksUtil.ReplaceBlock(game.Map?.Blocks, new List<int>() { id1.Value }, new List<int>() { id2.Value }, region);

                    game.AddBlocks(blocks);
                }
            }
        }

        private void BlockInfo(Game game)
        {
            if (game.Mode.Value != UserModes.BlockInfo)
            {
                BlockSelection.Reset();
                game.UserSelection.Reset();
                game.MeasureDistance.Reset();
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


    }
}