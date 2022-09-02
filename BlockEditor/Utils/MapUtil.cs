using BlockEditor.Models;
using BlockEditor.Views;
using BlockEditor.Views.Windows;
using DataAccess.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using System.Linq;
using LevelModel.Models.Components.Art;

namespace BlockEditor.Helpers
{
    public static class MapUtil
    {


        public static void Save(Map map)
        {
            if (map == null)
                return;

            var save    = new SaveMapWindow(map);
            var success = save.ShowDialog();

            if(success != true)
                return;

            if (!string.IsNullOrWhiteSpace(save.LocalFilepath))
            {
                SaveLocalFile(save.LocalFilepath, map);
            }
            else
            {
                map.Level.Title = save.MapTitle;
                (App.Current.MainWindow as MainWindow)?.TitleChanged(map.Level.Title);
                Upload(map, save.Publish, save.Newest);
            }
        }



        private static void Upload(Map map, bool publish, bool newest)
        {
            using(new TempCursor(Cursors.Wait))
            {
                try
                {
                    if (!CurrentUser.IsLoggedIn())
                    {
                        MessageUtil.ShowError("Requires user to login.");
                        return;
                    }

                    if (map.Blocks.StartBlocks.GetBlocks().Any(b => b.IsEmpty()))
                    {
                        MessageUtil.ShowError("Missing player start blocks..." 
                            + Environment.NewLine 
                            + Environment.NewLine 
                            + "Add the start blocks before you upload.");
                        return;
                    }

                    if(string.Equals("m3", map.Level.DataVersion, StringComparison.InvariantCultureIgnoreCase))
                        map.Level.DataVersion = "m4";

                    var data = map.ToPr2String(CurrentUser.Name, CurrentUser.Token, publish, false, newest);

                    var msg = DataAccess.PR2Accessor.Upload(data, (arg) =>
                    {
                        AskToOverwrite(arg);

                        if (arg.TryAgain)
                            arg.NewLevelData = map.ToPr2String(CurrentUser.Name, CurrentUser.Token, publish, true, newest);
                    });

                    if(msg != null && msg.Contains("message=", StringComparison.InvariantCultureIgnoreCase))
                        msg = msg.Replace("message=", "");

                    MessageUtil.ShowInfo(msg);
                }
                catch (Exception ex)
                {
                    MessageUtil.ShowError(ex.Message);
                }
            }
        }


        private static void SaveLocalFile(string filepath, Map map)
        {
            using (new TempCursor(Cursors.Wait))
            {
                try
                {
                    map.Level.Title = Path.GetFileNameWithoutExtension(filepath);
                    (App.Current.MainWindow as MainWindow)?.TitleChanged(map.Level.Title);

                    var data = map.ToPr2String(string.Empty, string.Empty, false, false, false);

                    File.WriteAllText(filepath, data);

                    MessageUtil.ShowInfo("The save was successful.");
                }
                catch (Exception ex)
                {
                    MessageUtil.ShowError(ex.Message);
                }
            }
        }

        private static void AskToOverwrite(LevelExistArg args)
        {
            var text = "You have another level with this title."
                + Environment.NewLine + Environment.NewLine
                + "Is it okej to overwrite the existing level with this save?";

            var result = UserQuestionWindow.Show(text, "Overwrite", false);

            args.TryAgain = result == UserQuestionWindow.QuestionResult.Yes;
        }


        public static void TestInTasTool(Map map)
        {
            if (map == null)
                return;

            var noteText  = map.Level.Note;
            var textArt1  = map.Level.TextArt1;
            var textArt2  = map.Level.TextArt2;
            var textArt3  = map.Level.TextArt3;
            var textArt0  = map.Level.TextArt0;
            var textArt00 = map.Level.TextArt00;

            try
            {
                map.Level.Note      = string.Empty;
                map.Level.TextArt1  = new List<TextArt>();
                map.Level.TextArt2  = new List<TextArt>();
                map.Level.TextArt3  = new List<TextArt>();
                map.Level.TextArt0  = new List<TextArt>();
                map.Level.TextArt00 = new List<TextArt>();

                var content  = map.ToPr2String(string.Empty, string.Empty, false, false, false);
                var filepath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";

                File.WriteAllText(filepath, content);

                StartTasTool(filepath);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
            finally
            {
                map.Level.Note      = noteText;
                map.Level.TextArt1  = textArt1;
                map.Level.TextArt2  = textArt2;
                map.Level.TextArt3  = textArt3;
                map.Level.TextArt0  = textArt0;
                map.Level.TextArt00 = textArt00;
            }
        }

        private static void StartTasTool(string levelFilepath)
        {
            var exePath = Path.Combine(Directory.GetCurrentDirectory(), "Dependencies", "TAS", "TAS.exe");
            var proc    = new Process();

            proc.StartInfo.FileName         =  exePath;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            proc.StartInfo.Arguments        = "\"" + levelFilepath+ "\"" + " " + true.ToString(CultureInfo.InvariantCulture);

            proc.Start();
        }

        public static Pen GetGridPen(Color mapBackground)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            if (mapBackground.R < 128)
                r = 255;
            if (mapBackground.G < 128)
                g = 255;
            if (mapBackground.B < 128)
                b = 255;

            return new Pen(Color.FromArgb(77, r, g, b), 1);
        }

        public static Brush GetSelectionBrush(Color c)
        {
            var brush = new SolidBrush(Color.FromArgb(50, c));

            return brush;
        }

        public static Pen GetSelectionPen()
        {
            int r = 100;
            int g = 200;
            int b = 255;

            return new Pen(Color.FromArgb(80, r, g, b), 1);
        }

        public static List<SimpleBlock> GetFloodFill(Map map, MyPoint? startPoint, int id, MyRegion region)
        {
            var result  = new List<SimpleBlock>();
            var blocks  = new Stack<MyPoint>();

            if (map == null || startPoint == null)
                return result;

            blocks.Push(startPoint.Value);
            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);
            var visited    = new List<MyPoint>();
            var startBlock = map.Blocks.GetBlock(startPoint);
            var maxBlocks  = Math.Min(Blocks.LIMIT - map.Blocks.BlockCount, 5_001);
            var shownWarning = false;

            if (region != null && region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            while (blocks.Count > 0)
            {
                var point = blocks.Pop();

                if(!shownWarning && result.Count >= 1000)
                {
                    shownWarning = true;
                    var r = UserQuestionWindow.ShowWarning("Over 1000 blocks has been added." 
                        + Environment.NewLine + Environment.NewLine
                        + "Do you wish to continue?", false);

                    if (r != UserQuestionWindow.QuestionResult.Yes)
                        return new List<SimpleBlock>();
                }

                if(maxBlocks <= result.Count)
                    throw new Exception("Too many blocks...only use the flood-fill tool in a closed region." 
                        + Environment.NewLine + Environment.NewLine
                        + "Operation has been cancelled.");

                if(visited.Contains(point))
                    continue;

                visited.Add(point);

                if (point.X < lowerLimit.X || point.X >= upperLimit.X)
                    continue;

                if (point.Y < lowerLimit.Y || point.Y >= upperLimit.Y)
                    continue;

                var currentBlock = map.Blocks.GetBlock(point.X, point.Y);

                if (currentBlock.ID != startBlock.ID)
                    continue;

                result.Add(new SimpleBlock(id, point));
                blocks.Push(new MyPoint(point.X - 1, point.Y));
                blocks.Push(new MyPoint(point.X + 1, point.Y));
                blocks.Push(new MyPoint(point.X, point.Y - 1));
                blocks.Push(new MyPoint(point.X, point.Y + 1));
            }

            return result;
        }

        public static List<SimpleBlock> ReplaceBlock(Map map, List<int> replace, List<int> add, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            if (map == null || replace == null || add == null || replace.Count != add.Count)
                return result;

            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);
            var notFound   = -1;

            if (region != null && region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            for (int x = lowerLimit.X; x < upperLimit.X; x++)
            {
                for (int y = lowerLimit.Y; y < upperLimit.Y; y++)
                {
                    var b = map.Blocks.GetBlock(x, y);
                    var index = replace.IndexOf(b.ID);

                    if(index != notFound)
                        result.Add(new SimpleBlock(add[index], x, y));
                }
            }

            return result;
        }

        public static List<SimpleBlock> RemoveBlocks(Map map, List<int> ids, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            if (map == null || ids == null)
                return result;

            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);

            if (region != null && region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            for (int x = lowerLimit.X; x < upperLimit.X; x++)
            {
                for (int y = lowerLimit.Y; y < upperLimit.Y; y++)
                {
                    var b = map.Blocks.GetBlock(x, y);

                    if (ids.Contains(b.ID))
                        result.Add(b);
                }
            }

            return result;
        }
    }
}
