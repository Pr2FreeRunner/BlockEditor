using System;
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
        public Game Game { get; }

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
            }
        }

        public BitmapImage MapContent
        {
            get => Game.GameImage?.GetImage(); 
        }

        public BlockSelection BlockSelection { get; }

        public bool IsSelectionMode => Mode == UserMode.Selection;
        public bool IsFillMode {
            get { return Mode == UserMode.Fill; }
            set { Mode = value ? UserMode.Fill : UserMode.None; }
        }


        public bool IsOverwrite {
            get { return Game.Map?.Blocks?.Overwrite ?? false; }
            set { Game.Map.Blocks.Overwrite = value; RaisePropertyChanged(); }
        }


        public RelayCommand StartPositionCommand { get; }
        public RelayCommand FillCommand { get; }
        public RelayCommand SelectCommand { get; }

        public MapViewModel(Action cleanBlockSelection)
        {
            Game = new Game();
            Mode = UserMode.None;

            BlockSelection       = new BlockSelection(cleanBlockSelection);
            StartPositionCommand = new RelayCommand((_) => Game.GoToStartPosition());
            FillCommand          = new RelayCommand((_) => OnFillClick());
            SelectCommand        = new RelayCommand((_) => BlockSelection.SelectionActivation());
            BlockSelection.OnSelectionClick += OnSelectionClick;
            Game.Engine.OnFrame += OnFrameUpdate;
        }


        #region Events
       
        private void OnSelectionClick()
        {
            if(Mode != UserMode.Selection)
            {
                Mode = UserMode.Selection;
                BlockSelection?.Clean();
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
                BlockSelection?.Clean(false);
            }
            else
            {
                Mode = UserMode.None;
            }
        }


        public void OnCleanUserMode()
        {
            BlockSelection?.Clean();
            Mode = UserMode.None;
        }

        public void OnFrameUpdate()
        {
            new FrameUpdate(Game, _mousePosition, BlockSelection);

            RaisePropertyChanged(nameof(MapContent));
        }

        internal void OnSelectedBlockID(int? id)
        {
            BlockSelection.SelectedBlock = id;
            BlockSelection.SelectedBlocks = null;

            if(id != null && Mode != UserMode.Fill)
                Mode = UserMode.AddBlock;
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p     = MyUtils.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Mode)
            {
                case UserMode.AddBlock:

                    if (e.ChangedButton == MouseButton.Right)
                        Game.DeleteBlock(index);
                    else if (e.ChangedButton == MouseButton.Left)
                        Game.AddBlock(index, BlockSelection.SelectedBlock);
                    break;

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

                case UserMode.AddSelection:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    Game.AddSelection(index, BlockSelection.SelectedBlocks);
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
                        var startId = Game.Map.Blocks.GetBlockId(index);

                        if(startId != null && !Game.Map.Blocks.Overwrite)
                            throw new Exception("Enable 'Overwrite' option for this to work.");

                        Game.AddBlocks(MapUtil.GetFloodFill(Game.Map, index, selectedId.Value));
                    }
                    break;

                case UserMode.None:
                    if (e.ChangedButton == MouseButton.Right)
                        Game.DeleteBlock(index);

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
            switch (Mode)
            {
                case UserMode.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    var pos   = MyUtils.GetPosition(sender as IInputElement, e);
                    var index = Game.GetMapIndex(pos);
                    
                    if(index != null)
                        index = new MyPoint(index.Value.X + 1, index.Value.Y + 1);

                    BlockSelection.UserSelection.OnMouseUp(pos, index);
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
