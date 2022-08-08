﻿using LevelModel.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlockEditor.Views.Controls
{

    public partial class ItemBlockOptionsControl : UserControl
    {

        private readonly List<CheckBox> _checkboxes = new List<CheckBox>();

        public event Action<string> OnBlockOptionChanged;

        public ItemBlockOptionsControl()
        {
            InitializeComponent();
            Init();
        }


        public void SetColumnCount(int count)
        {
            ItemGrid.Columns = count;
        }

        public void SetBlockOptions(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            var separators = new[] { ',', '-' };

            var split = input.Split(separators);

            foreach(var s in split)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;

                if(!int.TryParse(s.Trim(), out var id))
                    continue;

                var cb = _checkboxes.Where(x => x.Tag is int tag && tag == id).FirstOrDefault();

                if (cb == null)
                    continue;

                cb.IsChecked = true; 
            }
        }

        private void Init()
        {
            ItemGrid.Columns = 3;

            foreach (var item in GetItems())
            {
                var cb = new CheckBox();
                cb.Content = item.Name;
                cb.Tag = item.ID;
                cb.FontSize = 14;
                cb.HorizontalAlignment = HorizontalAlignment.Left;
                cb.Margin = new Thickness(5);
                cb.Checked   += Item_CheckedChange;
                cb.Unchecked += Item_CheckedChange;

                _checkboxes.Add(cb);
                ItemGrid.Children.Add(cb);
            }
        }

        private void Item_CheckedChange(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();
            var separator = "-";
            var first = true;

            foreach(var cb in _checkboxes)
            {
                if (cb?.IsChecked == null || cb.IsChecked == false)
                    continue;

                if (first)
                    first = false;
                else
                    builder.Append(separator);

                builder.Append(cb.Tag);
            }

            OnBlockOptionChanged?.Invoke(builder.ToString());
        }

        public IEnumerable<Item> GetItems()
        {
            var minId = 1;
            var maxId = 9;

            for (int i = minId; i <= maxId; i++)
            {
                var item = new Item(i);
                
                if (string.IsNullOrWhiteSpace(item.Name))
                    continue;

                yield return item;
            }
        }

    }
}
