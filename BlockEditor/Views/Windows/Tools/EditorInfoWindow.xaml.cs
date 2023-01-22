using BlockEditor.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BlockEditor.Views.Windows
{
    public partial class EditorInfoWindow : ToolWindow
    {
        public EditorInfoWindow()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            var culture = CultureInfo.InvariantCulture;

            tbFps.Text = GameEngine.FPS.ToString(culture);
            tbSize.Text = Blocks.SIZE.ToString(culture);
            tbLimit.Text = Blocks.LIMIT.ToString(culture);
            tbPlayTime.Text = GetPlayTime(culture);
            tbVersion.Text = MySettings.BlockEditorVersion;
            cbTas.IsChecked = MySettings.MaxOneTasRunning;

            BlockImage0.Source = GetImage(Key.D0);
            BlockImage1.Source = GetImage(Key.D1);
            BlockImage2.Source = GetImage(Key.D2);
            BlockImage3.Source = GetImage(Key.D3);
            BlockImage4.Source = GetImage(Key.D4);
            BlockImage5.Source = GetImage(Key.D5);
            BlockImage6.Source = GetImage(Key.D6);
            BlockImage7.Source = GetImage(Key.D7);
            BlockImage8.Source = GetImage(Key.D8);
            BlockImage9.Source = GetImage(Key.D9);
        }

        private string GetPlayTime(CultureInfo culture)
        {
            var endTime = DateTime.UtcNow;
            var timeDiff = endTime - MainWindow.StartTime;
            var playTime = MySettings.PlayTime + (int)Math.Round(timeDiff.TotalMinutes);
            var result = "";

            if(playTime > 60)
                result += ((int)Math.Floor(playTime / 60.0)).ToString(culture) + "h & ";

            var mins = (int)Math.Floor(playTime % 60.0);

            if(mins <= 1)
                result += mins.ToString(culture) + "min";
            else
                result += mins.ToString(culture) + "mins";

            return result;
        }

        private BitmapSource GetImage(Key k)
        {
            var id = MySettings.GetBlockId(k);

            if (id == null)
                return null;

            var block = BlockImages.GetImageBlock(BlockImages.BlockSize.Zoom100, id);
            return block?.Image;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void AddKey(Key k)
        {
            var id = SelectBlockWindow.Show("HotKey: " + k.ToString(), true);

            if (id == null)
                return;

            MySettings.SetBlockId(k, id.Value);
            Init();
        }


        private void Block0_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D0);
        }

        private void Block1_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D1);
        }

        private void Block2_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D2);
        }

        private void Block3_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D3);
        }

        private void Block4_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D4);
        }

        private void Block5_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D5);
        }

        private void Block6_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D6);
        }

        private void Block7_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D7);

        }

        private void Block8_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D8);
        }

        private void Block9_Click(object sender, MouseButtonEventArgs e)
        {
            AddKey(Key.D9);
        }

        private void cbTas_Checked(object sender, RoutedEventArgs e)
        {
            if(cbTas?.IsChecked == null)
                return;

            MySettings.MaxOneTasRunning = cbTas.IsChecked.Value;
        }
    }
}
