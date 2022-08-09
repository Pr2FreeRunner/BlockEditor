using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using BlockEditor.Models;
using static BlockEditor.Models.Hat;

namespace BlockEditor.Views.Controls
{

    public partial class HatsControl : UserControl
    {

        private readonly List<CheckBox> _checkboxes = new List<CheckBox>();

        public event Action<List<int>> OnHatChanged;

        public HatsControl()
        {
            InitializeComponent();
            Init();
        }


        public void SetColumnCount(int count)
        {
            ItemGrid.Columns = count;
        }

        public void SetBadHats(List<int> ids)
        {
            if (ids == null)
                return;

            foreach(var id in ids)
            {
                var cb = _checkboxes.Where(x => x.Tag is int tag && tag == id).FirstOrDefault();

                if (cb == null)
                    continue;

                cb.IsChecked = false; 
            }
        }

        private void Init()
        {
            ItemGrid.Columns = 3;

            foreach (Hats i in Hat.GetAllHats())
            {
                var cb = new CheckBox();
                var hat = new Hat((int)i);

                cb.Content = hat.Name;
                cb.Tag = hat.ID;
                cb.FontSize = 14;
                cb.IsChecked = true;
                cb.Margin = new Thickness(5);
                cb.Checked   += hat_CheckedChange;
                cb.Unchecked += hat_CheckedChange;

                _checkboxes.Add(cb);
                ItemGrid.Children.Add(cb);
            }
        }

        private void hat_CheckedChange(object sender, RoutedEventArgs e)
        {
            var hats = new List<int>();

            foreach(var cb in _checkboxes)
            {
                if (cb?.IsChecked == null || cb.IsChecked == true)
                    continue;

                if (!(cb.Tag is int id))
                    continue;

                hats.Add(id);
            }

            OnHatChanged?.Invoke(hats);
        }

    }
}
