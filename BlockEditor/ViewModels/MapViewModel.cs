using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using BlockEditor.Views.Windows;
using Builders.DataStructures.DTO;
using LevelModel.Models.Components;

using static BlockEditor.Models.BlockImages;
using static BlockEditor.Models.UserMode;

namespace BlockEditor.ViewModels
{

    public class MapViewModel : NotificationObject
    {

        private MyPoint? _mousePosition;

        public BitmapImage MapContent
        {
            get => Game.GameImage?.GetImage();
        }

        public Game Game { get; }

        public bool IsOverwrite
        {
            get { return Game.Map?.Blocks.Overwrite ?? false; }
            set { Game.Map.Blocks.Overwrite = value; RaisePropertyChanged(); }
        }


        public RelayCommand NavigatorCommand { get; }
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
        public RelayCommand DeleteRegionCommand { get; }
        public RelayCommand DeleteBlockOptionCommand { get; }
        public RelayCommand ReverseHorizontalArrowsCommand { get; }
        public RelayCommand ReverseVerticalArrowsCommand { get; }
        public RelayCommand DistanceCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand DeselectCommand { get; }

        public RelayCommand BuildMenuCommand { get; }
        public RelayCommand TransformMenuCommand { get; }
        public RelayCommand DeleteMenuCommand { get; }
        public RelayCommand EditMenuCommand { get; }
        public RelayCommand InfoMenuCommand { get; }
        public RelayCommand AdvancedMenuCommand { get; }
        public RelayCommand ReverseArrowsMenuCommand { get; }



        public MapViewModel()
        {
            Game = new Game();

            FillCommand = new RelayCommand((_) => FloodFill());
            SelectCommand = new RelayCommand((_) => Selection());
            AddShapeCommand = new RelayCommand((_) => AddShape(), (_) => Game.UserSelection.HasSelectedRegion);
            BlockInfoCommand = new RelayCommand((_) => BlockInfo());
            MapInfoCommand = new RelayCommand((_) => MapInfo());
            BlockCountCommand = new RelayCommand((_) => BlockCount());
            ReplaceCommand = new RelayCommand((_) => Replace());
            AddImageCommand = new RelayCommand((_) => AddImage());
            RotateCommand = new RelayCommand((_) => Rotate());
            VerticalFlipCommand = new RelayCommand((_) => VerticalFlip());
            HorizontalFlipCommand = new RelayCommand((_) => HorizontalFlip());
            DeleteBlockTypeCommand = new RelayCommand((_) => DeleteBlockType());
            SettingsCommand = new RelayCommand((_) => EditorSettings());
            ConnectTeleportsCommand = new RelayCommand((_) => ConnectTeleports());
            MoveRegionCommand = new RelayCommand((_) => MoveRegion());
            DeleteRegionCommand = new RelayCommand((_) => DeleteRegion());
            DistanceCommand = new RelayCommand((_) => Distance());
            DeleteCommand = new RelayCommand((_) => Delete());
            DeselectCommand = new RelayCommand((_) => Deselect(), (_) => DeselectCanExecute());
            NavigatorCommand = new RelayCommand((_) => Navigator(SimpleBlock.None));
            DeleteBlockOptionCommand = new RelayCommand((_) => DeleteBlockOption());
            ReverseHorizontalArrowsCommand = new RelayCommand((_) => ReverseHorizontalArrows());
            ReverseVerticalArrowsCommand = new RelayCommand((_) => ReverseVerticalArrows());

            BuildMenuCommand = MenuCommand(BuildMenu);
            TransformMenuCommand = MenuCommand(TransformMenu);
            DeleteMenuCommand = MenuCommand(DeleteMenu);
            EditMenuCommand = MenuCommand(EditMenu);
            AdvancedMenuCommand = MenuCommand(AdvancedMenu);
            InfoMenuCommand = MenuCommand(InfoMenu);
            ReverseArrowsMenuCommand = MenuCommand(ReverseArrowsMenu);

            Game.Engine.OnFrame += OnFrameUpdate;
        }



        #region Menu

        private RelayCommand MenuCommand(Action menu)
        {
            return new RelayCommand((_) =>
            {
                Game.Mode.UpdateGuiState();
                menu?.Invoke();
                Game.Mode.UpdateGuiState();
            });
        }

        private void BuildMenu()
        {
            var w = new MenuWindow("Build Tools");

            w.AddOption("Add Shape", AddShapeCommand);
            w.AddOption("Add Image", AddImageCommand);
            w.AddOption("Bucket Flood Fill", FillCommand);

            w.ShowDialog();
            w.Execute();
        }

        private void EditMenu()
        {
            var w = new MenuWindow("Edit Tools");

            w.AddOption("Move Region", MoveRegionCommand);
            w.AddOption("Replace Block Type", ReplaceCommand);
            w.AddOption("Reverse Arrows", ReverseArrowsMenuCommand);
            w.ShowDialog();
            w.Execute();
        }

        private void AdvancedMenu()
        {
            var w = new MenuWindow("Advanced Tools");

            w.AddOption("Connect Teleports", ConnectTeleportsCommand);
            w.AddOption("Measure Distance", DistanceCommand);

            w.ShowDialog();
            w.Execute();
            Game.Mode.UpdateGuiState();
        }

        private void ReverseArrowsMenu()
        {
            var w = new MenuWindow("Reverse Arrows");

            w.AddOption("Left/Right Arrows", ReverseHorizontalArrowsCommand);
            w.AddOption("Up/Down Arrows", ReverseVerticalArrowsCommand);

            w.ShowDialog();
            w.Execute();
            Game.Mode.UpdateGuiState();
        }

        private void InfoMenu()
        {
            Game.Mode.UpdateGuiState();
            var w = new MenuWindow("Info Tools");

            w.AddOption("Block Count", BlockCountCommand);
            w.AddOption("Block Info", BlockInfoCommand);
            w.AddOption("Editor Info", SettingsCommand);
            w.AddOption("Map Info", MapInfoCommand);

            w.ShowDialog();
            w.Execute();
            Game.Mode.UpdateGuiState();
        }

        private void TransformMenu()
        {
            Game.Mode.UpdateGuiState();
            var w = new MenuWindow("Transform Tools");

            w.AddOption("Rotate", RotateCommand);
            w.AddOption("Horizontal Flip", HorizontalFlipCommand);
            w.AddOption("Vertical Flip", VerticalFlipCommand);

            w.ShowDialog();
            w.Execute();
        }

        private void DeleteMenu()
        {
            var w = new MenuWindow("Delete Tools");

            w.AddOption("Block Type", DeleteBlockTypeCommand);
            w.AddOption("Block Option", DeleteBlockOptionCommand);
            w.AddOption("Delete Blocks", DeleteCommand);
            w.AddOption("Delete Region", DeleteRegionCommand);

            w.ShowDialog();
            w.Execute();
            Game.Mode.UpdateGuiState();
        }

        #endregion


        #region Tool Commands

        private void ReverseHorizontalArrows()
        {
            CleanUserMode(true, false);

            using (new TempOverwrite(Game.Map.Blocks, true))
            using (new TempCursor(Cursors.Wait))
            {
                var remove = new List<int> { Block.ARROW_LEFT, Block.ARROW_RIGHT };
                var add    = new List<int> { Block.ARROW_RIGHT, Block.ARROW_LEFT };
                var blocks = MapUtil.ReplaceBlock(Game.Map, remove, add, Game.UserSelection.MapRegion);

                Game.AddBlocks(blocks);
            }
        }

        private void ReverseVerticalArrows()
        {
            CleanUserMode(true, false);

            using (new TempOverwrite(Game.Map.Blocks, true))
            using (new TempCursor(Cursors.Wait))
            {
                var remove = new List<int> { Block.ARROW_UP, Block.ARROW_DOWN };
                var add = new List<int> { Block.ARROW_DOWN, Block.ARROW_UP };
                var blocks = MapUtil.ReplaceBlock(Game.Map, remove, add, Game.UserSelection.MapRegion);

                Game.AddBlocks(blocks);
            }
        }

        private bool DeselectCanExecute()
        {
            if (BlockSelection.SelectedBlocks != null)
                return true;

            if (BlockSelection.SelectedBlock != null)
                return true;

            if (BlockSelection.SelectedBlockOption != null)
                return true;

            if (Game.Mode.Value != UserModes.None)
                return true;

            if (Mouse.OverrideCursor != null)
                return true;

            if (Game.UserSelection.HasSelectedRegion)
                return true;

            if (Game.UserSelection.MapRegion.Point1 != null)
                return true;

            if (Game.MeasureDistance.MapPoint1 != null)
                return true;

            return false;
        }

        private void Deselect()
        {
            CleanUserMode(true, true);
        }

        private void Delete()
        {
            BlockSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Game.Mode.Value != UserModes.Delete)
            {
                Game.Mode.Value = UserModes.Delete;
            }
            else
            {
                Game.Mode.Value = UserModes.None;
            };
        }

        private void Distance()
        {
            BlockSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Game.Mode.Value != UserModes.Distance)
            {
                Game.Mode.Value = UserModes.Distance;
            }
            else
            {
                Game.Mode.Value = UserModes.None;
            };
        }

        private void Navigator(SimpleBlock block)
        {
            CleanUserMode(false, true);
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
                Game.GoToStartPosition(id.Value);
            else
                NavigatorWindow.Show(Game, filter, id.Value, block.Position);
        }

        private void Selection()
        {
            BlockSelection.Reset();
            Game.UserSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Game.Mode.Value != UserModes.Selection)
            {
                Game.Mode.Value = UserModes.Selection;
            }
            else
            {
                Game.Mode.Value = UserModes.None;
            }
        }

        private void ConnectTeleports()
        {
            BlockSelection.Reset();
            Game.UserSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Game.Mode.Value != UserModes.ConnectTeleports)
            {
                Models.ConnectTeleports.Start();
                Game.Mode.Value = UserModes.ConnectTeleports;
            }
            else
            {
                var blocks = Models.ConnectTeleports.End(out var addMore);

                if (addMore)
                {
                    Game.Mode.Value = UserModes.ConnectTeleports; // updates GUI
                    return;
                }

                using(new TempOverwrite(Game.Map.Blocks, true))
                {
                    Game.AddBlocks(blocks);
                    Game.Mode.Value = UserModes.None;
                }
            }
        }

        private void FloodFill()
        {
            if (Game.Mode.Value != UserModes.Fill)
            {
                BlockSelection.Reset();
                Game.MeasureDistance.Reset();
                Game.Mode.Value = UserModes.Fill;
            }
            else
            {
                Game.Mode.Value = UserModes.None;
            }
        }

        private void EditorSettings()
        {
            CleanUserMode(true, true);

            new SettingsWindow().ShowDialog();

            Game.Mode.Value = UserModes.None;
        }

        private void VerticalFlip()
        {
            if (BlockSelection.VerticalFlipCommand.CanExecute(null))
            {
                BlockSelection.VerticalFlipCommand.Execute(null);
            }
            else if (Game.UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (Game.UserSelection.SelectedRegionContainsBlocks(Game.Map))
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
                    Game.Engine.PauseConfirmed();
                    Game.Map.Blocks.VerticalFlip();
                    Game.GoToStartPosition();
                    Game.Engine.Pause = false;
                }
            }
        }

        private void MoveRegion()
        {
            CleanUserMode(true, false);

            var w = new EditArtWindow(Game.Map, Game.UserSelection.MapRegion, true);

            w.ShowDialog();

            using(new TempCursor(Cursors.Wait)) 
            { 
                if (w.BlocksToRemove != null && w.BlocksToRemove.Any())
                    Game.RemoveBlocks(w.BlocksToRemove);

                if (w.BlocksToAdd != null && w.BlocksToAdd.Any())
                    Game.AddBlocks(w.BlocksToAdd);
            }
        }

        private void DeleteRegion()
        {
            CleanUserMode(true, false);

            var w = new EditArtWindow(Game.Map, Game.UserSelection.MapRegion, false);

            w.ShowDialog();

            using (new TempCursor(Cursors.Wait))
            {
                if (w.BlocksToRemove != null && w.BlocksToRemove.Any())
                    Game.RemoveBlocks(w.BlocksToRemove);

                if (w.BlocksToAdd != null && w.BlocksToAdd.Any())
                    Game.AddBlocks(w.BlocksToAdd);
            }
        }

        private void Rotate()
        {
            if (BlockSelection.RotateCommand.CanExecute(null))
            {
                BlockSelection.RotateCommand.Execute(null);
            }
            else if (Game.UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (Game.UserSelection.SelectedRegionContainsBlocks(Game.Map))
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
                    Game.Engine.PauseConfirmed();
                    Game.Map.Blocks.Rotate();
                    Game.GoToStartPosition();
                    Game.Engine.Pause = false;
                }
            }
        }

        private void HorizontalFlip()
        {
            if (BlockSelection.HorizontalFlipCommand.CanExecute(null))
            {
                BlockSelection.HorizontalFlipCommand.Execute(null);
            }
            else if (Game.UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (Game.UserSelection.SelectedRegionContainsBlocks(Game.Map))
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
                    Game.Engine.PauseConfirmed();
                    Game.Map.Blocks.HorizontalFlip();
                    Game.GoToStartPosition();
                    Game.Engine.Pause = false;
                }
            }
        }

        private void AddShape()
        {
            CleanUserMode(true, false);

            var selectedId = SelectBlockWindow.Show("Add Shape", false);
            var region = Game.UserSelection.MapRegion;

            if (selectedId == null)
                return;

            if (!ShapeBuilderUtil.PickShape())
                return;

            var blocks = ShapeBuilderUtil.Build(Game.Map, selectedId.Value, region);

            if (blocks != null && !blocks.Any() && region != null && region.IsComplete() && !Game.Map.Blocks.Overwrite && ShapeBuilderUtil.Probablity != 100)
                throw new OverwriteException();

            Game.AddBlocks(blocks);
            BlockSelection.Reset();
        }

        private void AddImage(MyPoint? p = null)
        {
            CleanUserMode(true, true);

            var w = new ImageToBlocksWindow(p);
            var r = w.ShowDialog();

            if (w.GetPosition)
            {
                Game.Mode.Value = UserModes.GetPosition;
                Game.GetPosition = (x) => AddImage(x);
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
                    var blocks = MyConverters.ToBlocks(pr2Blocks, out var blocksOutsideBoundries).GetBlocks();
                    var position = blocks.First().Position;

                    MyUtils.BlocksOutsideBoundries(blocksOutsideBoundries);
                    Game.AddBlocks(blocks);
                    Game.GoToPosition(position);
                }
                else
                {
                    w.ShiftPosition(level.DrawArt0);
                    w.ShiftPosition(level.DrawArt1);

                    Game.Map.Level.DrawArt1.AddRange(level.DrawArt1);
                    Game.Map.Level.DrawArt0.AddRange(level.DrawArt0);

                    MessageUtil.ShowInfo("The image has been added to the map." + Environment.NewLine + Environment.NewLine + "Note:  Art is not visible inside the Block Editor.");
                }
            }
        }

        private void DeleteBlockType()
        {
            CleanUserMode(true, false);

            var region = Game.UserSelection.MapRegion;
            var id1 = SelectBlockWindow.Show("Block Type to Remove:", false);

            if (id1 == null)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                var blocks = MapUtil.RemoveBlocks(Game.Map, new List<int>() { id1.Value }, region);

                Game.RemoveBlocks(blocks);
            }
        }

        private void DeleteBlockOption()
        {
            CleanUserMode(true, false);

            var region = Game.UserSelection.MapRegion;
            var remove = new List<SimpleBlock>();
            var add = new List<SimpleBlock>();

            using (new TempCursor(Cursors.Wait))
            {
                foreach(var oldBlock in MapUtil.GetBlocks(Game.Map, region))
                {
                    if (oldBlock.IsEmpty())
                        continue;

                    var newBlock = new SimpleBlock(oldBlock.ID, oldBlock.Position.Value, string.Empty);
                    remove.Add(oldBlock);
                    add.Add(newBlock);
                }
                

                Game.RemoveBlocks(remove);
                Game.AddBlocks(add);
            }
        }

        private void Replace()
        {
            CleanUserMode(true, false);


            using (new TempOverwrite(Game.Map.Blocks, true))
            {
                var region = Game.UserSelection.MapRegion;
                var id1 = SelectBlockWindow.Show("Block to Replace:", false);

                if (id1 == null)
                    return;

                var id2 = SelectBlockWindow.Show("Block to Add:", false);
                if (id2 == null)
                    return;

                using (new TempCursor(Cursors.Wait))
                {
                    var blocks = MapUtil.ReplaceBlock(Game.Map, new List<int>() { id1.Value }, new List<int>() { id2.Value }, region);

                    Game.AddBlocks(blocks);
                }
            }
        }

        private void BlockInfo()
        {
            if (Game.Mode.Value != UserModes.BlockInfo)
            {
                BlockSelection.Reset();
                Game.UserSelection.Reset();
                Game.MeasureDistance.Reset();
                Game.Mode.Value = UserModes.BlockInfo;
            }
            else
            {
                Game.Mode.Value = UserModes.None;
            }
        }

        private void BlockCount()
        {
            CleanUserMode(true, true);

            var w = new BlockCountWindow(Game.Map);
            w.ShowDialog();
        }

        private void MapInfo()
        {
            CleanUserMode(true, true);

            var w = new MapInfoWindow(Game.Map, Game.Engine.RefreshGui);
            w.ShowDialog();
        }

        public void CleanUserMode(bool blockSelection, bool userSelection)
        {
            if (blockSelection)
                BlockSelection.Reset();

            if (userSelection)
                Game.UserSelection.Reset();

            Game.Mode.Value = UserModes.None;
            Mouse.OverrideCursor = null;
            Game.MeasureDistance.Reset();
        }
        
        #endregion


        #region Events

        public void OnFrameUpdate()
        {
            new FrameUpdate(Game, _mousePosition);

            RaisePropertyChanged(nameof(MapContent));
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Game.Mode.Value)
            {
                case UserModes.Delete:
                    if (Game.UserSelection.MapRegion.IsInside(index))
                        Game.DeleteSelection(Game.UserSelection.MapRegion);
                    else
                        Game.DeleteBlock(index);
                    break;

                case UserModes.Distance:
                    Game.MeasureDistance.Reset();
                    Game.MeasureDistance.MapPoint1 = index;
                    Game.MeasureDistance.ImagePoint1 = p;
                    break;

                case UserModes.GetPosition:
                    if (Game.GetPosition != null)
                        Game.GetPosition(index);
                    else
                        Game.Mode.Value = UserModes.None;
                    break;

                case UserModes.ConnectTeleports:
                    Models.ConnectTeleports.Add(Game.Map.Blocks.GetBlock(index));
                    break;

                case UserModes.Selection:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Game.UserSelection.OnMouseDown(p, index);
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        if (!Game.UserSelection.MapRegion.IsInside(index))
                            break;

                        Game.DeleteSelection(Game.UserSelection.MapRegion);
                        CleanUserMode(true, true);
                    }

                    break;

                case UserModes.BlockInfo:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if (p == null)
                        break;

                    bool navigate = false;
                    using (new TempCursor(null))
                    {
                        var w = new BlockOptionWindow(Game.Map, index, Game.Engine.RefreshGui);
                        w.ShowDialog();

                        navigate = w.StartNavigation;
                    }

                    if (navigate)
                    {
                        Game.Mode.Value = UserModes.None;
                        Navigator(Game.Map.Blocks.GetBlock(index));
                    }
                    break;

                case UserModes.Fill:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if (Game.UserSelection.HasSelectedRegion && !Game.UserSelection.MapRegion.IsInside(index))
                    {
                        MessageUtil.ShowInfo("You clicked outside the selected region."
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Either remove the selected region or click inside it.");

                        return;
                    }

                    var selectedId = SelectBlockWindow.Show("Flood Fill", false);

                    if (selectedId == null)
                        return;

                    if (Block.IsStartBlock(selectedId))
                        throw new Exception("Flood fill with start block is not allowed.");

                    using (new TempCursor(Cursors.Wait))
                    {
                        var b = Game.Map.Blocks.GetBlock(index);

                        if (!b.IsEmpty() && !Game.Map.Blocks.Overwrite)
                            throw new OverwriteException();

                        var region = Game.UserSelection.HasSelectedRegion ? Game.UserSelection.MapRegion : null;
                        Game.AddBlocks(MapUtil.GetFloodFill(Game.Map, index, selectedId.Value, region));
                    }
                    break;

                default:
                    if (e.ChangedButton == MouseButton.Right)
                    {
                        Game.DeleteBlock(index);
                    }
                    else if (e.ChangedButton == MouseButton.Left)
                    {
                        if (BlockSelection.SelectedBlocks != null)
                            Game.AddSelection(index, BlockSelection.SelectedBlocks);
                        else if (BlockSelection.SelectedBlock != null)
                            Game.AddBlock(index, BlockSelection.SelectedBlock);
                        else
                        {
                            var b = Game.Map.Blocks.GetBlock(index, true);

                            if (!b.IsEmpty())
                            {
                                Game.DeleteBlock(b);
                                BlockSelection.SelectedBlock = b.ID;
                                BlockSelection.SelectedBlockOption = b.Options;
                                Game.Mode.Value = UserModes.MoveBlock;
                            }

                        }
                    }
                    break;
            }
        }

        public void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(_mousePosition);

            switch (Game.Mode.Value)
            {
                case UserModes.Delete:
                    if (e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (Game.UserSelection.MapRegion.IsInside(index))
                            Game.DeleteSelection(Game.UserSelection.MapRegion);
                        else
                            Game.DeleteBlock(index);
                    }
                    break;

                case UserModes.MoveBlock:
                    break;

                default:
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        Game.DeleteBlock(index);
                    }
                    else if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (BlockSelection.SelectedBlocks != null)
                            Game.AddSelection(index, BlockSelection.SelectedBlocks);
                        else
                            Game.AddBlock(index, BlockSelection.SelectedBlock);
                    }
                    break;
            }
        }

        internal void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var p = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Game.Mode.Value)
            {
                case UserModes.Distance:
                    Game.MeasureDistance.MapPoint2 = index;
                    Game.MeasureDistance.ImagePoint2 = p;

                    MessageUtil.ShowInfo(Game.MeasureDistance.GetDistance());
                    break;

                case UserModes.MoveBlock:
                    if (BlockSelection.SelectedBlock != null)
                        Game.AddBlock(index, BlockSelection.SelectedBlock.Value, BlockSelection.SelectedBlockOption);

                    BlockSelection.Reset();
                    Game.Mode.Value = UserModes.None;
                    break;


                case UserModes.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    Game.UserSelection.OnMouseUp(p, index);

                    if (MySettings.FirstUserSelection
                        && Game.UserSelection.HasSelectedRegion
                        && Game.UserSelection.SelectedRegionContainsBlocks(Game.Map))
                    {
                        MessageUtil.ShowInfo("Hint:  To copy the blocks inside the selected region press Ctrl + C");
                        MySettings.FirstUserSelection = false;
                    }
                    break;
            }
        }

        public void OnSizeChanged(int width, int height)
        {
            if (Game.GameImage != null)
                Game.GameImage.Dispose();

            Game.Camera.ScreenSize = new MyPoint(width, height);
            Game.GameImage = new GameImage(width, height);  // thread safe?
        }

        public void OnLoadMap(Map map)
        {
            if (map == null)
                return;

            Game.Engine.PauseConfirmed();

            var size = Game.Map.BlockSize;
            Game.Map = map;
            Game.Map.BlockSize = size;

            Game.UserOperations.Clear();
            (App.Current.MainWindow as MainWindow)?.TitleChanged(Game.Map.Level.Title);
            CleanUserMode(true, true);
            Game.GoToStartPosition();

            MyUtils.BlocksOutsideBoundries(map.BlocksOutsideBoundries);

            Game.Engine.Pause = false;
        }

        public void OnZoomChanged(BlockSize size)
        {
            var halfScreenX = Game.GameImage.Width / 2;
            var halfScreenY = Game.GameImage.Height / 2;

            var cameraPosition = new MyPoint(Game.Camera.Position.X, Game.Camera.Position.Y);
            var middleOfScreen = new MyPoint(cameraPosition.X + halfScreenX, cameraPosition.Y + halfScreenY);

            var currentIndex = Game.Map.GetMapIndex(middleOfScreen);
            var currentSize = Game.Map.BlockSize;

            Game.Map.BlockSize = size;

            var x = currentIndex.X * Game.Map.BlockPixelSize - halfScreenX;
            var y = currentIndex.Y * Game.Map.BlockPixelSize - halfScreenY;

            Game.Camera.Position = new MyPoint(x, y);
        }

        #endregion
    }
}
