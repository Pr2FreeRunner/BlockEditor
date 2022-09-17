using System;
using System.Windows;
using System.Windows.Input;
using BlockEditor.Models;
using BlockEditor.Utils;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Net.NetworkInformation;
using BlockEditor.Helpers;

namespace BlockEditor.Views.Windows
{
    public partial class NavigatorWindow : ToolWindow
    {

        private readonly  Game _game;
        private readonly int _blockId;
        private readonly List<MyPoint> _positions;

        private int _index;


        private NavigatorWindow(Game game, List<MyPoint> positions, int blockId, MyPoint? p)
        {
            InitializeComponent();

            _game = game;
            _positions = positions;
            _blockId = blockId;

            Init(p);
            MyUtil.SetPopUpWindowPosition(this);
        }


        public static void Show(Game game, Predicate<SimpleBlock> filter, int blockId, MyPoint? p)
        {
            if (game == null || filter == null)
                throw new Exception("Something went wrong...");

            var blocks = new List<SimpleBlock>();
            var positions = new List<MyPoint>();

            using (new TempCursor(Cursors.Wait))
            {
                blocks = game.Map.Blocks.GetBlocks().RemoveEmpty().Where(b => filter(b)).ToList();
                positions = blocks.Select(b => b.Position.Value).ToList();
            }

            if (positions.Count == 0)
            {
                MessageUtil.ShowInfo("The block was not found.");
                return;
            }

            new NavigatorWindow(game, positions, blockId, p).ShowDialog();

        }


        private void Init(MyPoint? p)
        {
            BlockImage.Source = BlockImages.GetImageBlock(BlockImages.BlockSize.Zoom160, _blockId)?.Image;

            if(p != null)
            {
                var index = _positions.IndexOf(p.Value);

                if (index != -1)
                    _index = index;
            }

            UpdateGui();
        }

        private void UpdateGui()
        {
            if (_index < 0 || _index >= _positions.Count)
                _index = 0;

            tbIndex.Text = (_index+ 1) + " / " + _positions.Count;
            btnNext.IsEnabled = btnPrevious.IsEnabled = _positions.Count > 1;
            
            if(_positions.Any())
            {
                var pos = _positions[_index];
                _game.GoToPosition(pos);
                lblPosX.Text = pos.X.ToString();
                lblPosY.Text = pos.Y.ToString();
            }
            else
            {
                lblPosX.Text =  "";
                lblPosY.Text = "";
            }

            _game.Engine.RefreshGui();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnPrevious(object sender, RoutedEventArgs e)
        {
            if (_positions == null)
                return;

            _index--;

            if (_index < 0)
                _index = Math.Max(0, _positions.Count - 1);

            UpdateGui();
        }

        private void OnNext(object sender, RoutedEventArgs e)
        {
            if (_positions == null)
                return;

            _index++;

            if (_index >= _positions.Count)
                _index = 0;

            UpdateGui();
        }
    }
}
