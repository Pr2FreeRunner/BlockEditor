using System;
using System.Collections.Generic;
using System.Drawing;
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
using SkiaSharp;
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


            Game.Engine.OnFrame += OnFrameUpdate;
        }


        #region Events

        private void OnNavigatorClick(SimpleBlock block)
        {
            Mode.Value = UserModes.None;
            BlockSelection.Reset();
            UserSelection.Reset();
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

            if (Mode.Value != UserModes.ConnectTeleports)
            {
                ConnectTeleports.Start();
                Mode.Value = UserModes.ConnectTeleports;
            }
            else
            {
                var overwrite = Game.Map.Blocks.Overwrite;

                try
                {
                    var blocks = ConnectTeleports.End();
                    Game.Map.Blocks.Overwrite = true;
                    Game.AddBlocks(blocks);
                    Mode.Value = UserModes.None;
                }
                finally
                {
                    Game.Map.Blocks.Overwrite = overwrite;
                }
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

        public void OnSettingsClick()
        {
            BlockSelection.Reset();
            UserSelection.Reset();
            Mode.Value = UserModes.Settings;

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

                var r2 = UserQuestionWindow.Show("Do you wish to convert Up-Arrow blocks to Down-Arrow blocks and vice versa?", "Vertical Flip", false);

                using (new TempCursor(Cursors.Wait))
                {
                    Game.Engine.PauseConfirmed();
                    Game.Map.Blocks.VerticalFlip();

                    if (r2 == UserQuestionWindow.QuestionResult.Yes)
                    {
                        var replace = new List<int>() { Block.ARROW_UP, Block.ARROW_DOWN };
                        var add = new List<int>() { Block.ARROW_DOWN, Block.ARROW_UP };
                        var blocks = MapUtil.ReplaceBlock(Game.Map, replace, add, null);
                        Game.AddBlocks(blocks);
                    }

                    Game.GoToStartPosition();
                    Game.Engine.Pause = false;
                }
            }
        }

        public void OnRotateClick()
        {
            if (BlockSelection.RotateCommand.CanExecute(null))
            {
                BlockSelection.RotateCommand.Execute(null);
            }
            else if(UserSelection.HasSelectedRegion)
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
                var r1 = UserQuestionWindow.Show("Do you wish horizontally flip the map?", "Horizontal Flip", false);

                if (r1 != UserQuestionWindow.QuestionResult.Yes)
                    return;

                var r2 = UserQuestionWindow.Show("Do you wish to convert Left-Arrow blocks to Right-Arrow blocks and vice versa?", "Horizontal Flip", false);

                using (new TempCursor(Cursors.Wait))
                {
                    Game.Engine.PauseConfirmed();
                    Game.Map.Blocks.HorizontalFlip();

                    if (r2 == UserQuestionWindow.QuestionResult.Yes)
                    {
                        var replace = new List<int>() { Block.ARROW_LEFT, Block.ARROW_RIGHT };
                        var add = new List<int>() { Block.ARROW_RIGHT, Block.ARROW_LEFT };
                        var blocks = MapUtil.ReplaceBlock(Game.Map, replace, add, null);
                        Game.AddBlocks(blocks);
                    }

                    Game.GoToStartPosition();
                    Game.Engine.Pause = false;
                }
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
                var blocks = MyConverters.ToBlocks(pr2Blocks, out var blocksOutsideBoundries).GetBlocks();
                var position = blocks.First().Position;

                MyUtils.BlocksOutsideBoundries(blocksOutsideBoundries);
                Game.AddBlocks(blocks);
                Game.GoToPosition(position);
            }
        }

        public void OnDeleteBlockClick()
        {
            BlockSelection.Reset();

            var region = UserSelection.MapRegion;
            var id1 = SelectBlockWindow.Show("Block Type to Remove:", false);

            if (id1 == null)
                return;

            using (new TempCursor(Cursors.Wait))
            {
                var blocks = MapUtil.RemoveBlocks(Game.Map, new List<int>() { id1.Value }, region);

                Game.RemoveBlocks(blocks);
                BlockSelection.Reset();
                UserSelection.Reset();
                Mode.Value = UserModes.None;
            }
        }

        public void OnReplaceClick()
        {
            var currentOverWrite = Game.Map.Blocks.Overwrite;
            Game.Map.Blocks.Overwrite = true;

            try
            {
                BlockSelection.Reset();
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
                    BlockSelection.Reset();
                    UserSelection.Reset();
                    Mode.Value = UserModes.None;
                }
            }
            finally
            {
                Game.Map.Blocks.Overwrite = currentOverWrite;
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
            if (clearBlockSelection)
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
                        OnCleanUserMode(true);
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
                        else if(BlockSelection.SelectedBlock != null)
                            Game.AddBlock(index, BlockSelection.SelectedBlock);
                        else
                        {
                            var b = Game.Map.Blocks.GetBlock(index, true);

                            if(!b.IsEmpty())
                            {
                                Game.DeleteBlock(b);
                                BlockSelection.SelectedBlock = b.ID;
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
                case UserModes.MoveBlock:
                    Game.AddBlock(index, BlockSelection.SelectedBlock);
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
