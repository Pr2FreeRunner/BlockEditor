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

namespace BlockEditor.Helpers
{
    public static class MapUtil
    {


        public static void Save(Map map)
        {
            if (map == null)
                return;

            var save    = new WindowSaveMap(map);
            var success = save.ShowDialog();

            if(success != true)
                return;

            if (!string.IsNullOrWhiteSpace(save.LocalFilepath))
            {
                SaveLocalFile(save.LocalFilepath, map);
            }
            else
            {
                map.Title = save.MapTitle;
                Upload(map, save.Publish);
            }
        }

        private static void Upload(Map map, bool publish)
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

                    var data = map.ToPr2String(CurrentUser.Name, CurrentUser.Token, publish, false);

                    var msg = DataAccess.PR2Accessor.Upload(data, (arg) =>
                    {
                        AskToOverwrite(arg);

                        if (arg.TryAgain)
                            arg.NewLevelData = map.ToPr2String(CurrentUser.Name, CurrentUser.Token, publish, true);
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
                    map.Title = Path.GetFileNameWithoutExtension(filepath);

                    var data = map.ToPr2String(string.Empty, string.Empty, false);

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

            var result = UserQuestionWindow.Show(text, "Confirm", false);

            args.TryAgain = result == UserQuestionWindow.QuestionResult.Yes;
        }


        public static void TestInTasTool(Map map)
        {
            if (map == null)
                return;

            try
            {
                var content  = map.ToPr2String(string.Empty, string.Empty, false);
                var filepath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";

                File.WriteAllText(filepath, content);

                StartTasTool(filepath);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
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
            int r   = 0;
            int g   = 0;
            int b   = 0;

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

        public static List<SimpleBlock> GetRectangleFill(Map map, int id, MyRegion region)
        {
            var result    = new List<SimpleBlock>();
            var overwrite = map.Blocks.Overwrite;

            if (map == null || region == null || !region.IsComplete())
                return result;

            for (int x = region.Start.Value.X; x < region.End.Value.X; x++)
            {
                for (int y = region.Start.Value.Y; y < region.End.Value.Y; y++)
                {
                    if (overwrite)
                    {
                        result.Add(new SimpleBlock(id, x, y));
                    }
                    else
                    {
                        var currentId = map.Blocks.GetBlockId(x, y);

                        if (currentId == null)
                            result.Add(new SimpleBlock(id, x, y));
                    }
                }
            }

            return result;
        }

        public static List<SimpleBlock> GetFloodFill(Map map, MyPoint? startPoint, int id)
        {
            var result  = new List<SimpleBlock>();
            var blocks  = new Stack<MyPoint>();

            if (map == null || startPoint == null)
                return result;

            blocks.Push(startPoint.Value);
            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);
            var visited    = new List<MyPoint>();
            var startId    = map.Blocks.GetBlockId(startPoint);
            var maxBlocks  = Blocks.LIMIT - map.Blocks.BlockCount;

            while (blocks.Count > 0)
            {
                var point = blocks.Pop();

                if(maxBlocks <= result.Count)
                    throw new BlockLimitException("Operation Canceled" + Environment.NewLine + Environment.NewLine);

                if(visited.Contains(point))
                    continue;

                visited.Add(point);

                if (point.X < lowerLimit.X && point.X >= upperLimit.X)
                    continue;

                if (point.Y < lowerLimit.Y && point.Y >= upperLimit.Y)
                    continue;

                var currentId = map.Blocks.GetBlockId(point.X, point.Y);

                if (currentId != startId)
                    continue;

                result.Add(new SimpleBlock(id, point));
                blocks.Push(new MyPoint(point.X - 1, point.Y));
                blocks.Push(new MyPoint(point.X + 1, point.Y));
                blocks.Push(new MyPoint(point.X, point.Y - 1));
                blocks.Push(new MyPoint(point.X, point.Y + 1));
            }

            return result;
        }
    }
}
