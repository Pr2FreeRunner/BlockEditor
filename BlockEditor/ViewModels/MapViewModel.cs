using System;
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

namespace BlockEditor.ViewModels
{
    public class MapViewModel : NotificationObject
    {

        private MyPoint? _mousePosition;
        private UserMode _mode;

        public UserMode Mode { 
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;

                RaisePropertyChanged(nameof(IsSelectionMode));
                RaisePropertyChanged(nameof(IsFillMode));
                RaisePropertyChanged(nameof(IsAddShapeMode));
                RaisePropertyChanged(nameof(IsBlockInfoMode));

                if (_mode != UserMode.Fill)
                    Mouse.OverrideCursor = null;
            }
        }

        public BitmapImage MapContent
        {
            get => Game.GameImage?.GetImage(); 
        }

        public BlockSelection BlockSelection { get; }
        public Game Game { get; }

        public bool IsSelectionMode => Mode == UserMode.Selection;
        public bool IsAddShapeMode => Mode == UserMode.AddShape;
        public bool IsFillMode => Mode == UserMode.Fill; 
        public bool IsBlockInfoMode => Mode == UserMode.BlockInfo; 

        public bool IsOverwrite {
            get { return Game.Map?.Blocks?.Overwrite ?? false; }
            set { Game.Map.Blocks.Overwrite = value; RaisePropertyChanged(); }
        }

        public RelayCommand StartPositionCommand { get; }
        public RelayCommand FillCommand { get; }
        public RelayCommand SelectCommand { get; }
        public RelayCommand AddShapeCommand { get; }
        public RelayCommand BlockInfoCommand { get; }


        public MapViewModel(Action cleanBlockSelection)
        {
            Game = new Game();
            Mode = UserMode.None;

            BlockSelection       = new BlockSelection(cleanBlockSelection);
            StartPositionCommand = new RelayCommand((_) => Game.GoToStartPosition());
            FillCommand          = new RelayCommand((_) => OnFillClick());
            SelectCommand        = new RelayCommand((_) => OnSelectionClick());
            AddShapeCommand      = new RelayCommand((_) => OnAddShapeClick());
            BlockInfoCommand     = new RelayCommand((_) => OnBlockInfoClick());

            BlockSelection.OnSelectionClick += OnSelectionClick;
            Game.Engine.OnFrame += OnFrameUpdate;
        }


        #region Events
       
        private void OnSelectionClick()
        {
            if(Mode != UserMode.Selection)
            {
                BlockSelection.Reset(); 
                Mode = UserMode.Selection;
            }
            else
            {
                Mode = UserMode.None;
            }         
        }

        public void OnFillClick()
        {
            if (Mode != UserMode.Fill)
            {
                Mode = UserMode.Fill;
                Mouse.OverrideCursor = Cursors.UpArrow;
                BlockSelection?.Reset(false);
            }
            else
            {
                Mouse.OverrideCursor = null;
                Mode = UserMode.None;
            }
        }

        public void OnAddShapeClick()
        {
            if (Mode != UserMode.AddShape)
            {
                BlockSelection?.Reset(false);
                Mode = UserMode.AddShape;
            }
            else
            {
                Mode = UserMode.None;
            }
        }

        public void OnBlockInfoClick()
        {
            if (Mode != UserMode.BlockInfo)
            {
                BlockSelection?.Reset();
                Mode = UserMode.BlockInfo;
            }
            else
            {
                Mode = UserMode.None;
            }
        }

        internal void OnSelectedBlockID(int? id)
        {
            BlockSelection.SelectedBlocks = null;
            BlockSelection.SelectedBlock = id;

            if(id != null && Mode != UserMode.Fill && Mode != UserMode.AddShape)
                Mode = UserMode.AddBlock;
        }

        public void OnCleanUserMode()
        {
            BlockSelection?.Reset();
            Mode = UserMode.None;
            Mouse.OverrideCursor = null;
        }

        public void OnFrameUpdate()
        {
            new FrameUpdate(Game, _mousePosition, BlockSelection);

            RaisePropertyChanged(nameof(MapContent));
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p     = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Mode)
            {
                case UserMode.Selection:

                    if (e.LeftButton == MouseButtonState.Pressed) 
                    { 
                        BlockSelection.UserSelection.OnMouseDown(p, index);
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    { 
                        var start = BlockSelection.UserSelection.MapRegion.Start;
                        var end   = BlockSelection.UserSelection.MapRegion.End;

                        if(BlockSelection.UserSelection.MapRegion.IsInside(index))
                            break;

                        Game.DeleteSelection(start, end);
                        OnCleanUserMode();
                    }

                    break;

                case UserMode.AddShape:
                    if (e.LeftButton != MouseButtonState.Pressed)
                        break;

                    BlockSelection.UserSelection.OnMouseDown(p, index);
                    break;

                case UserMode.AddSelection:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    Game.AddSelection(index, BlockSelection.SelectedBlocks);
                    break;

                case UserMode.BlockInfo:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if(p == null)
                        break;

                    new BlockOptionWindow(Game.Map.Blocks.GetBlock(index), index).ShowNextToClick();
                    break;

                case UserMode.Fill:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    var selectedId = BlockSelection.SelectedBlock;
                    
                    if(selectedId == null)
                        throw new Exception("Select a block to flood fill.");

                    if(Block.IsStartBlock(selectedId))
                        throw new Exception("Flood fill with start block is not allowed.");

                    using(new TempCursor(Cursors.Wait)) 
                    {
                        var b = Game.Map.Blocks.GetBlock(index);

                        if(!b.IsEmpty() && !Game.Map.Blocks.Overwrite)
                            throw new OverwriteException();

                        Game.AddBlocks(MapUtil.GetFloodFill(Game.Map, index, selectedId.Value));
                    }
                    break;

                default:
                    if (e.ChangedButton == MouseButton.Right)
                        Game.DeleteBlock(index);
                    else if (e.ChangedButton == MouseButton.Left)
                        Game.AddBlock(index, BlockSelection.SelectedBlock);
                    break;
            }
        }

        public void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(_mousePosition);

            switch (Mode)
            {
                case UserMode.AddBlock:
                    if (e.RightButton == MouseButtonState.Pressed)
                        Game.DeleteBlock(index);
                    else if (e.LeftButton == MouseButtonState.Pressed)
                        Game.AddBlock(index, BlockSelection.SelectedBlock);
                    break;


                case UserMode.AddSelection:
                    if (e.LeftButton != MouseButtonState.Pressed)
                        break;

                    Game.AddSelection(index, BlockSelection.SelectedBlocks);
                    break;
            }
        }

        internal void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var p = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if(p == null || index == null)
                return;

            switch (Mode)
            {
                case UserMode.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    BlockSelection.UserSelection.OnMouseUp(p, index);
                    break;

                case UserMode.AddShape:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    BlockSelection.UserSelection.OnMouseUp(p, index);

                    var selectedId = BlockSelection.SelectedBlock;
                    var region     = BlockSelection.UserSelection.MapRegion.Copy();
                    BlockSelection.UserSelection.Reset();

                    if (selectedId == null)
                        throw new Exception("Select a block to add a shape.");

                    if(!ShapeBuilderUtil.PickShape())
                        break;

                    var blocks = ShapeBuilderUtil.Build(Game.Map, selectedId.Value, region);

                    if(blocks != null && !blocks.Any() && region != null && region.IsComplete() && !Game.Map.Blocks.Overwrite)
                        throw new OverwriteException();

                    Game.AddBlocks(blocks);
                    break;
            }
        }

        public void OnSizeChanged(int width, int height)
        {
            if(Game.GameImage != null)
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
            OnCleanUserMode();
            Game.GoToStartPosition();

            Game.Engine.Pause = false;
        }

        public void OnZoomChanged(BlockSize size)
        {
            var halfScreenX = Game.GameImage.Width  / 2;
            var halfScreenY = Game.GameImage.Height / 2;

            var cameraPosition = new MyPoint(Game.Camera.Position.X, Game.Camera.Position.Y);
            var middleOfScreen = new MyPoint(cameraPosition.X + halfScreenX, cameraPosition.Y + halfScreenY);

            var currentIndex = Game.Map.GetMapIndex(middleOfScreen);
            var currentSize  = Game.Map.BlockSize;

            Game.Map.BlockSize = size;

            var x = currentIndex.X * Game.Map.BlockPixelSize - halfScreenX;
            var y = currentIndex.Y * Game.Map.BlockPixelSize - halfScreenY;

            Game.Camera.Position = new MyPoint(x, y);
        }

        #endregion

    }
}
