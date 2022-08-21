using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using BlockEditor.Views.Windows;
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
            get { return Game.Map?.Blocks?.Overwrite ?? false; }
            set { Game.Map.Blocks.Overwrite = value; RaisePropertyChanged(); }
        }

        public RelayCommand StartPositionCommand { get; }
        public RelayCommand FillCommand { get; }
        public RelayCommand SelectCommand { get; }
        public RelayCommand AddShapeCommand { get; }
        public RelayCommand BlockInfoCommand { get; }
        public RelayCommand MapInfoCommand { get; }
        public RelayCommand BlockCountCommand { get; }
        public RelayCommand ReplaceCommand { get; }
        public RelayCommand AddImageCommand { get; }


        public MapViewModel()
        {
            Game = new Game();
            Mode = new UserMode();

            UserSelection = new UserSelection();
            StartPositionCommand = new RelayCommand((_) => Game.GoToStartPosition());
            FillCommand = new RelayCommand((_) => OnFillClick());
            SelectCommand = new RelayCommand((_) => OnSelectionClick());
            AddShapeCommand = new RelayCommand((_) => OnAddShapeClick(), (_) => UserSelection.HasSelectedRegion);
            BlockInfoCommand = new RelayCommand((_) => OnBlockInfoClick());
            MapInfoCommand = new RelayCommand((_) => OnMapInfoClick());
            BlockCountCommand = new RelayCommand((_) => OnBlockCountClick());
            ReplaceCommand = new RelayCommand((_) => OnReplaceClick());
            AddImageCommand = new RelayCommand((_) => OnAddImageClick());

            Game.Engine.OnFrame += OnFrameUpdate;
        }


        #region Events

        private void OnSelectionClick()
        {
            if (Mode.Value != UserModes.Selection)
            {
                BlockSelection.Reset();
                UserSelection.Reset();

                Mode.Value = UserModes.Selection;
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnFillClick()
        {
            if (Mode.Value != UserModes.Fill)
            {
                BlockSelection.Reset();
                Mode.Value = UserModes.Fill;
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnAddShapeClick()
        {
            BlockSelection.Reset();

            var selectedId = SelectBlockWindow.Show("Add Shape", false);
            var region = UserSelection.MapRegion;

            if (selectedId == null)
                return;

            if (!ShapeBuilderUtil.PickShape())
                return;

            var blocks = ShapeBuilderUtil.Build(Game.Map, selectedId.Value, region);

            if (blocks != null && !blocks.Any() && region != null && region.IsComplete() && !Game.Map.Blocks.Overwrite)
                throw new OverwriteException();

            Game.AddBlocks(blocks);
            BlockSelection.Reset();
        }

        public void OnAddImageClick()
        {
            BlockSelection.Reset(); 
            UserSelection.Reset();
            Mode.Value = UserModes.None;

            var w = new ImageToBlocksWindow();
            var r = w.ShowDialog();

            if (r != true || w.BuildInfo == null)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                var pr2Blocks = Builders.PR2Builder.BuildLevel(w.BuildInfo).Blocks.Skip(8).ToList();
                ImageToBlocksWindow.ShiftPosition(pr2Blocks);
                var blocks  = MyConverters.ToBlocks(pr2Blocks, out var blocksOutsideBoundries).GetBlocks();
                var position = blocks.First().Position;

                MyUtils.BlocksOutsideBoundries(blocksOutsideBoundries);
                Game.AddBlocks(blocks);
                Game.GoToPosition(position);
            }
        }

        public void OnReplaceClick()
        {
            BlockSelection.Reset();

            if (!Game.Map.Blocks.Overwrite)
                throw new OverwriteException();

            var region = UserSelection.MapRegion;
            var id1 = SelectBlockWindow.Show("Block to Replace:", false);

            if (id1 == null)
                return;

            var id2 = SelectBlockWindow.Show("Block to Add:", false);
            if (id2 == null)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                var blocks = MapUtil.ReplaceBlock(Game.Map, id1.Value, id2.Value, region);

                Game.AddBlocks(blocks);
                BlockSelection.Reset();
                UserSelection.Reset();
            }
        }

        public void OnBlockInfoClick()
        {
            if (Mode.Value != UserModes.BlockInfo)
            {
                BlockSelection.Reset();
                UserSelection.Reset();
                Mode.Value = UserModes.BlockInfo;
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnBlockCountClick()
        {
            if (Mode.Value != UserModes.BlockCount)
            {
                BlockSelection.Reset();
                UserSelection.Reset();

                if (Game.Map == null)
                    return;

                Mode.Value = UserModes.BlockCount;
                var w = new BlockCountWindow(Game.Map);
                w.Closing += (s, e) => { if (Mode.Value == UserModes.BlockCount) Mode.Value = UserModes.None; };
                w.ShowDialog();
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnMapInfoClick()
        {
            if (Mode.Value != UserModes.MapInfo)
            {
                BlockSelection.Reset();
                UserSelection.Reset();

                if (Game.Map == null)
                    return;

                Mode.Value = UserModes.MapInfo;
                var w = new MapInfoWindow(Game.Map, Game.Engine.RefreshGui);
                w.Closing += (s, e) => { if (Mode.Value == UserModes.MapInfo) Mode.Value = UserModes.None; };
                w.ShowDialog();
            }
            else
            {
                Mode.Value = UserModes.None;
            }
        }

        public void OnCleanUserMode(bool clearBlockSelection)
        {
            if(clearBlockSelection)
                BlockSelection.Reset();

            UserSelection.Reset();
            Mode.Value = UserModes.None;
            Mouse.OverrideCursor = null;
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
                        OnCleanUserMode(true);
                    }

                    break;

                case UserModes.BlockInfo:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if (p == null)
                        break;

                    using (new TempCursor(null))
                        new BlockOptionWindow(Game.Map, index).ShowDialog();
                    break;

                case UserModes.Fill:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

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
                        if(BlockSelection.SelectedBlocks != null)
                            Game.AddSelection(index, BlockSelection.SelectedBlocks);
                        else
                            Game.AddBlock(index, BlockSelection.SelectedBlock);
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
                case UserModes.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    UserSelection.OnMouseUp(p, index);
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

            Game.Engine.Pause = true;
            Thread.Sleep(GameEngine.FPS * 5); // make sure engine has been stopped

            var size = Game.Map.BlockSize;
            Game.Map = map;
            Game.Map.BlockSize = size;

            Game.UserOperations.Clear();
            (App.Current.MainWindow as MainWindow)?.TitleChanged(Game.Map.Level.Title);
            OnCleanUserMode(true);
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
