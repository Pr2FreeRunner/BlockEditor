using BlockEditor.Helpers;
using BlockEditor.Models;
using LevelModel.Models.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{

    public partial class BlockCountWindow : ToolWindow
    {
        public BlockCountWindow(Map map)
        {
            InitializeComponent();

            if(map == null)
                throw new ArgumentException("map");

            using (new TempCursor(Cursors.Wait))
                Init(map);
        }

        private void Init(Map map)
        {
            var dim = BlocksUtil.GetDimensions(map?.Blocks);

            tbTotal.Text  = map.Blocks.BlockCount.ToString(CultureInfo.InvariantCulture);
            tbWidth.Text  = dim.Item1.ToString(CultureInfo.InvariantCulture);
            tbHeight.Text = dim.Item2.ToString(CultureInfo.InvariantCulture);

            SetSpecificBlockCount(map);
        }

        private void SetSpecificBlockCount(Map map)
        {
            var builder = new StringBuilder();
            var blocks  = GetBlockCount(map).ToList().OrderByDescending(x => x.Item1).ToList();
            var first   = true;

            var maxCount = blocks.Any() ? blocks.Max(t => t.Item1) : 0;
            var maxName  = blocks.Any() ? blocks.Max(t => t.Item2.Length) : 0;

            var countPadding = maxCount < 100 ? 2 : maxCount < 1000 ? 3 : maxCount < 10000 ? 4 : 5;
            var namePadding  = maxName + 2;

            foreach (var tuple in blocks)
            {
                if(first)
                    first = false;
                else
                    builder.Append(Environment.NewLine);

                builder.Append((tuple.Item2 + ":").PadRight(namePadding) + tuple.Item1.ToString(CultureInfo.InvariantCulture).PadLeft(countPadding));
            }

            tbCount.Text = builder.ToString();
        }

        private IEnumerable<Tuple<int, string>> GetBlockCount(Map map)
        {
            var blocks = map.Blocks.GetBlocks(true).GroupBy(b => b.ID);

            for (int i = Block.BASIC_BROWN; i <= Block.MaxBlockId; i++)
            {
                var count = blocks.Where(g => g.Key == i).FirstOrDefault()?.Count() ?? 0;
                var name = Block.GetBlockName(i);

                if (count == 0 || string.IsNullOrWhiteSpace(name))
                    continue;

                if (name.StartsWith("start ", StringComparison.InvariantCultureIgnoreCase))
                    name = name.Substring(6);

                yield return new Tuple<int, string>(count, name);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
