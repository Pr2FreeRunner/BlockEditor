using BlockEditor.Models;
using BlockEditor.Utils;
using LevelModel.Models;
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

    public partial class BlockCountWindow : Window
    {
        public BlockCountWindow(Map map)
        {
            InitializeComponent();

            if(map == null)
                throw new ArgumentException("map");

            OpenWindows.Add(this);
            MyUtils.SetPopUpWindowPosition(this);

            tbTotal.Text = map.Blocks.BlockCount.ToString(CultureInfo.InvariantCulture);

           

            using (new TempCursor(Cursors.Wait))
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
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
