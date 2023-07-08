using BlockEditor.Helpers;
using BlockEditor.Utils;
using BlockEditor.ViewModels;
using LevelModel.Models.Components;
using System;
using System.Collections.Generic;
using System.Windows.Input;

using static BlockEditor.Models.UserMode;

namespace BlockEditor.Models
{
    public class Game : IDisposable
    {

        public GameEngine Engine { get; }
        
        public Camera Camera { get; set; }

        public Map Map { get; set; }

        public UserMode Mode { get; }

        public UserSelection UserSelection { get; }

        public Action<MyPoint?> GetPosition { get; set; }

        public UserOperations UserOperations { get; }

        public MeasureDistance MeasureDistance { get;  }

        public MyPoint? MousePosition { get; set; }

        private bool _disposedValue;

        public bool ShowArt 
        { 
            get { return MySettings.ShowArt && ShowArtIsEnabled(); }
            set { MySettings.ShowArt = value; }
        }

        public Game()
        {
            Mode = new UserMode();
            Map = new Map();
            Engine = new GameEngine();
            Camera = new Camera();
            UserOperations = new UserOperations();
            MeasureDistance = new MeasureDistance();
            UserSelection = new UserSelection(GetMapIndex);
        }


        public bool ShowArtIsEnabled()
        {
            return Map.BlockSize >= BlockImages.BlockSize.Zoom10;
        }

        public MyPoint? GetMapIndex(MyPoint? p)
        {
            if (p == null || Map == null)
                return  null;

            var x = p.Value.X + Camera.Position.X;
            var y = p.Value.Y + Camera.Position.Y;

            var pos = new MyPoint(x, y);
            return Map.GetMapIndex(pos);
        }

        public void AddBlock(MyPoint? index, int? id, string option = "", bool isMapIndex = false)
        {
            if (index == null || id == null || Map == null)
                return;

            var op = new AddBlockOperation(Map, new SimpleBlock(id.Value, index.Value, option));
            UserOperations.Execute(op);
        }

        public void AddBlocks(IEnumerable<SimpleBlock> blocks)
        {
            if (blocks == null || Map == null)
                return;

            if (!blocks.AnyBlocks())
                return;

            var op = new AddBlocksOperation(Map, blocks);
            UserOperations.Execute(op);
        }

        public void RemoveBlocks(IEnumerable<SimpleBlock> blocks)
        {
            if (Map == null)
                return;

            if (!blocks.AnyBlocks())
                return;

            var op = new DeleteBlocksOperation(Map, blocks);
            UserOperations.Execute(op);
        }

        public void DeleteBlocks(MyRegion region)
        {
            if (!region.IsComplete() || Map == null)
                return;

            var result = new List<SimpleBlock>();

            for (int x = region.Start.Value.X; x < region.End.Value.X; x++)
            {
                for (int y = region.Start.Value.Y; y < region.End.Value.Y; y++)
                {
                    var normalBlock = Map.Blocks.GetBlock(x, y, false);
                    var startBlocks = Map.Blocks.StartBlocks.GetBlocks(x, y);

                    if (!normalBlock.IsEmpty())
                        result.Add(normalBlock);

                    if (startBlocks != null)
                        result.AddRange(startBlocks.RemoveEmpty());
                }
            }

            if (!result.AnyBlocks())
                return;

            var op = new DeleteBlocksOperation(Map, result);
            UserOperations.Execute(op);
        }

        public void DeleteBlock(MyPoint? index)
        {
            if (index == null || Map == null)
                return;

            var block = Map.Blocks.GetBlock(index.Value.X, index.Value.Y);

            DeleteBlock(block);
        }

        public void DeleteBlock(SimpleBlock block)
        {
            if (block.IsEmpty())
                return;

            var op = new DeleteBlockOperation(Map, block);
            UserOperations.Execute(op);
        }

        public void GoToStartPosition(int id = Block.START_BLOCK_P1, bool showError = true)
        {
            if (Map == null)
                return;

            var p = Map.Blocks.StartBlocks.GetPosition(id);

            if (p == null)
            {
                if(showError)
                    MessageUtil.ShowWarning("The start block was not found.");

                return;
            }

            GoToPosition(p.Value);
        }

        public void GoToPosition(MyPoint? p)
        {
            if(p == null)
                return;

            var size = Map.BlockSize.GetPixelSize();
            var x = p.Value.X * size - (Camera.ScreenSize.X / 2);
            var y = p.Value.Y * size - (Camera.ScreenSize.Y / 2);

            Camera.Position = new MyPoint(x, y);
        }

        public void CleanUserMode(bool blockSelection, bool userSelection)
        {
            if (blockSelection)
                BlockSelection.Reset();

            if (userSelection)
                UserSelection.Reset();

            App.MyMainWindow?.CurrentMap?.ClearSidePanel();
            Mode.Value = UserModes.None;
            Mouse.OverrideCursor = null;
            MeasureDistance.Reset();
        }

        public void HorizontalFlipRegion(MyRegion region)
        {
            if(Map == null)
                return;

            var blocksToAdd = new List<SimpleBlock>();
            var blocksToRemove = new List<SimpleBlock>();

            foreach (var b in BlocksUtil.GetBlocks(Map?.Blocks, region))
            {
                if (b.IsEmpty())
                    continue;

                var relStart = b.Position.Value.X - region.Start.Value.X + 1;
                var point = new MyPoint(region.End.Value.X - relStart, b.Position.Value.Y);
                var block = new SimpleBlock(b.ID, point, b.Options);

                blocksToRemove.Add(b);
                blocksToAdd.Add(block);
            }

            RemoveBlocks(blocksToRemove);
            AddBlocks(blocksToAdd);
        }

        public void VerticalFlipRegion(MyRegion region)
        {
            if (Map == null)
                return;

            var blocksToAdd = new List<SimpleBlock>();
            var blocksToRemove = new List<SimpleBlock>();

            foreach (var b in BlocksUtil.GetBlocks(Map?.Blocks, region))
            {
                if (b.IsEmpty())
                    continue;

                var relStart = b.Position.Value.Y - region.Start.Value.Y + 1;
                var point = new MyPoint(b.Position.Value.X, region.End.Value.Y - relStart);
                var block = new SimpleBlock(b.ID, point, b.Options);

                blocksToRemove.Add(b);
                blocksToAdd.Add(block);
            }

            RemoveBlocks(blocksToRemove);
            AddBlocks(blocksToAdd);
        }

        public void HorizontalFlipMap()
        {
            if (Map == null)
                return;

            var blocksToAdd = new List<SimpleBlock>();
            var blocksToRemove = new List<SimpleBlock>();

            foreach (var b in Map.Blocks.GetBlocks(true))
            {
                if (b.IsEmpty())
                    continue;

                var point = new MyPoint(Blocks.SIZE - b.Position.Value.X, b.Position.Value.Y);
                var block = new SimpleBlock(b.ID, point, b.Options);

                blocksToRemove.Add(b);
                blocksToAdd.Add(block);
            }

            RemoveBlocks(blocksToRemove);
            AddBlocks(blocksToAdd);
        }

        public void VerticalFlipMap()
        {
            if (Map == null)
                return;

            var blocksToAdd = new List<SimpleBlock>();
            var blocksToRemove = new List<SimpleBlock>();

            foreach (var b in Map.Blocks.GetBlocks(true))
            {
                if (b.IsEmpty())
                    continue;

                var point = new MyPoint(b.Position.Value.X, Blocks.SIZE - b.Position.Value.Y);
                var block = new SimpleBlock(b.ID, point, b.Options);

                blocksToRemove.Add(b);
                blocksToAdd.Add(block);
            }

            RemoveBlocks(blocksToRemove);
            AddBlocks(blocksToAdd);
        }

        public void RotateMap()
        {
            if (Map == null)
                return;

            var blocksToAdd = new List<SimpleBlock>();
            var blocksToRemove = new List<SimpleBlock>();

            foreach (var b in Map.Blocks.GetBlocks(true))
            {
                if (b.IsEmpty())
                    continue;

                var point = new MyPoint(b.Position.Value.Y, Blocks.SIZE - b.Position.Value.X - 1);
                var block = new SimpleBlock(b.ID, point, b.Options);

                blocksToRemove.Add(b);
                blocksToAdd.Add(block);
            }

            RemoveBlocks(blocksToRemove);
            AddBlocks(blocksToAdd);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Map.Renderer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
