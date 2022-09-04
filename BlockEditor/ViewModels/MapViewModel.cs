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

        public UserMode Mode { get; }
        public UserSelection UserSelection { get; }

        public Game Game { get; }
        public bool IsOverwrite
        {
            get { return Game.Map?.Blocks.Overwrite ?? false; }
            set { Game.Map.Blocks.Overwrite = value; RaisePropertyChanged(); }
        }

        private Action<MyPoint?> _getPosition;

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
        public RelayCommand DeleteBlockCommand { get; }
        public RelayCommand SettingsCommand { get; }
        public RelayCommand ConnectTeleportsCommand { get; }
        public RelayCommand EditArtCommand { get; }
        public RelayCommand DistanceCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand DeselectCommand { get; }




        public MapViewModel()
        {
            Game = new Game();
            Mode = new UserMode();

            UserSelection = new UserSelection();
            NavigatorCommand = new RelayCommand((_) => OnNavigatorClick(SimpleBlock.None));
            FillCommand = new RelayCommand((_) => OnFillClick());
            SelectCommand = new RelayCommand((_) => OnSelectionClick());
            AddShapeCommand = new RelayCommand((_) => OnAddShapeClick(), (_) => UserSelection.HasSelectedRegion);
            BlockInfoCommand = new RelayCommand((_) => OnBlockInfoClick());
            MapInfoCommand = new RelayCommand((_) => OnMapInfoClick());
            BlockCountCommand = new RelayCommand((_) => OnBlockCountClick());
            ReplaceCommand = new RelayCommand((_) => OnReplaceClick());
            AddImageCommand = new RelayCommand((_) => OnAddImageClick());
            RotateCommand = new RelayCommand((_) => OnRotateClick());
            VerticalFlipCommand = new RelayCommand((_) => OnVerticalFlipClick());
            HorizontalFlipCommand = new RelayCommand((_) => OnHorizontalFlipClick());
            DeleteBlockCommand = new RelayCommand((_) => OnDeleteBlockClick());
            SettingsCommand = new RelayCommand((_) => OnSettingsClick());
            ConnectTeleportsCommand = new RelayCommand((_) => OnConnectTeleportsClick());
            EditArtCommand = new RelayCommand((_) => OnEditArtClick());
            DistanceCommand = new RelayCommand((_) => OnDistanceClick());
            DeleteCommand = new RelayCommand((_) => OnDeleteClick());
            DeselectCommand = new RelayCommand((_) => OnDeselectClick(), (_) => OnDeselectCanExecute());


            Game.Engine.OnFrame += OnFrameUpdate;
        }


        #region Events

        private bool OnDeselectCanExecute()
        {
            if (BlockSelection.SelectedBlocks != null)
                return true;

            if (BlockSelection.SelectedBlock != null)
                return true;

            if (BlockSelection.SelectedBlockOption != null)
                return true;

            if (Mode.Value != UserModes.None)
                return true;

            if (Mouse.OverrideCursor != null)
                return true;

            if (UserSelection.HasSelectedRegion)
                return true;

            if (UserSelection.MapRegion.Point1 != null)
                return true;

            if (Game.MeasureDistance.MapPoint1 != null)
                return true;

            return false;
        }

        private void OnDeselectClick()
        {
            OnCleanUserMode(true, true);
        }

        private void OnDeleteClick()
        {
            BlockSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Mode.Value != UserModes.Delete)
            {
                Mode.Value = UserModes.Delete;
            }
            else
            {
                Mode.Value = UserModes.None;
            };
        }

        private void OnDistanceClick()
        {
            BlockSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Mode.Value != UserModes.Distance)
            {
                Mode.Value = UserModes.Distance;
            }
            else
            {
                Mode.Value = UserModes.None;
            };
        }

        private void OnNavigatorClick(SimpleBlock block)
        {
            OnCleanUserMode(false, true);
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

        private void OnSelectionClick()
        {
            BlockSelection.Reset();
            UserSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Mode.Value != UserModes.Selection)
            {
                Mode.Value = UserModes.Selection;
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        private void OnConnectTeleportsClick()
        {
            BlockSelection.Reset();
            UserSelection.Reset();
            Game.MeasureDistance.Reset();

            if (Mode.Value != UserModes.ConnectTeleports)
            {
                ConnectTeleports.Start();
                Mode.Value = UserModes.ConnectTeleports;
            }
            else
            {
                var blocks = ConnectTeleports.End(out var addMore);

                if (addMore)
                {
                    Mode.Value = UserModes.ConnectTeleports; // updates GUI
                    return;
                }

                using(new TempOverwrite(Game.Map.Blocks, true))
                {
                    Game.AddBlocks(blocks);
                    Mode.Value = UserModes.None;
                }
            }
        }

        public void OnFillClick()
        {
            if (Mode.Value != UserModes.Fill)
            {
                BlockSelection.Reset();
                Game.MeasureDistance.Reset();
                Mode.Value = UserModes.Fill;
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnSettingsClick()
        {
            OnCleanUserMode(true, true);

            new SettingsWindow().ShowDialog();

            Mode.Value = UserModes.None;
        }

        public void OnVerticalFlipClick()
        {
            if (BlockSelection.VerticalFlipCommand.CanExecute(null))
            {
                BlockSelection.VerticalFlipCommand.Execute(null);
            }
            else if (UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (UserSelection.SelectedRegionContainsBlocks(Game.Map))
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

        public void OnEditArtClick()
        {
            OnCleanUserMode(true, false);

            var w = new EditArtWindow(Game.Map, UserSelection.MapRegion);

            w.ShowDialog();

            using(new TempCursor(Cursors.Wait)) 
            { 
                if (w.BlocksToRemove != null && w.BlocksToRemove.Any())
                    Game.RemoveBlocks(w.BlocksToRemove);

                if (w.BlocksToAdd != null && w.BlocksToAdd.Any())
                    Game.AddBlocks(w.BlocksToAdd);
            }

            if (!string.IsNullOrWhiteSpace(w.Message))
                MessageUtil.ShowInfo(w.Message);
        }

        public void OnRotateClick()
        {
            if (BlockSelection.RotateCommand.CanExecute(null))
            {
                BlockSelection.RotateCommand.Execute(null);
            }
            else if (UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (UserSelection.SelectedRegionContainsBlocks(Game.Map))
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

        public void OnHorizontalFlipClick()
        {
            if (BlockSelection.HorizontalFlipCommand.CanExecute(null))
            {
                BlockSelection.HorizontalFlipCommand.Execute(null);
            }
            else if (UserSelection.HasSelectedRegion)
            {
                string text = "";

                if (UserSelection.SelectedRegionContainsBlocks(Game.Map))
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

        public void OnAddShapeClick()
        {
            OnCleanUserMode(true, false);

            var selectedId = SelectBlockWindow.Show("Add Shape", false);
            var region = UserSelection.MapRegion;

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

        public void OnAddImageClick(MyPoint? p = null)
        {
            OnCleanUserMode(true, true);

            var w = new ImageToBlocksWindow(p);
            var r = w.ShowDialog();

            if (w.GetPosition)
            {
                Mode.Value = UserModes.GetPosition;
                _getPosition = (x) => OnAddImageClick(x);
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

        public void OnDeleteBlockClick()
        {
            OnCleanUserMode(true, false);

            var region = UserSelection.MapRegion;
            var id1 = SelectBlockWindow.Show("Block Type to Remove:", false);

            if (id1 == null)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                var blocks = MapUtil.RemoveBlocks(Game.Map, new List<int>() { id1.Value }, region);

                Game.RemoveBlocks(blocks);
            }
        }

        public void OnReplaceClick()
        {
            OnCleanUserMode(true, false);


            using (new TempOverwrite(Game.Map.Blocks, true))
            {
                var region = UserSelection.MapRegion;
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

        public void OnBlockInfoClick()
        {
            if (Mode.Value != UserModes.BlockInfo)
            {
                BlockSelection.Reset();
                UserSelection.Reset();
                Game.MeasureDistance.Reset();
                Mode.Value = UserModes.BlockInfo;
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnBlockCountClick()
        {
            OnCleanUserMode(true, true);

            var w = new BlockCountWindow(Game.Map);
            w.ShowDialog();
        }

        public void OnMapInfoClick()
        {
            OnCleanUserMode(true, true);

            var w = new MapInfoWindow(Game.Map, Game.Engine.RefreshGui);
            w.ShowDialog();
        }

        public void OnCleanUserMode(bool blockSelection, bool userSelection)
        {
            if (blockSelection)
                BlockSelection.Reset();

            if (userSelection)
                UserSelection.Reset();

            Mode.Value = UserModes.None;
            Mouse.OverrideCursor = null;
            Game.MeasureDistance.Reset();
        }

        public void OnFrameUpdate()
        {
            new FrameUpdate(Game, _mousePosition, UserSelection);

            RaisePropertyChanged(nameof(MapContent));
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Mode.Value)
            {
                case UserModes.Delete:
                    if (UserSelection.MapRegion.IsInside(index))
                        Game.DeleteSelection(UserSelection.MapRegion);
                    else
                        Game.DeleteBlock(index);
                    break;

                case UserModes.Distance:
                    Game.MeasureDistance.Reset();
                    Game.MeasureDistance.MapPoint1 = index;
                    Game.MeasureDistance.ImagePoint1 = p;
                    break;

                case UserModes.GetPosition:
                    if (_getPosition != null)
                        _getPosition(index);
                    else
                        Mode.Value = UserModes.None;
                    break;

                case UserModes.ConnectTeleports:
                    ConnectTeleports.Add(Game.Map.Blocks.GetBlock(index));
                    break;

                case UserModes.Selection:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        UserSelection.OnMouseDown(p, index);
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        if (!UserSelection.MapRegion.IsInside(index))
                            break;

                        Game.DeleteSelection(UserSelection.MapRegion);
                        OnCleanUserMode(true, true);
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
                        Mode.Value = UserModes.None;
                        OnNavigatorClick(Game.Map.Blocks.GetBlock(index));
                    }
                    break;

                case UserModes.Fill:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if (UserSelection.HasSelectedRegion && !UserSelection.MapRegion.IsInside(index))
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

                        var region = UserSelection.HasSelectedRegion ? UserSelection.MapRegion : null;
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
                                Mode.Value = UserModes.MoveBlock;
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

            switch (Mode.Value)
            {
                case UserModes.Delete:
                    if (e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (UserSelection.MapRegion.IsInside(index))
                            Game.DeleteSelection(UserSelection.MapRegion);
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

            switch (Mode.Value)
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
                    Mode.Value = UserModes.None;
                    break;


                case UserModes.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    UserSelection.OnMouseUp(p, index);

                    if (MySettings.FirstUserSelection
                        && UserSelection.HasSelectedRegion
                        && UserSelection.SelectedRegionContainsBlocks(Game.Map))
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
            OnCleanUserMode(true, true);
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
