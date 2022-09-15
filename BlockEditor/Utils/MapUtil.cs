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
using SkiaSharp;
using BlockEditor.Utils;
using static Builders.DataStructures.DTO.ImageDTO;
using System.Net;

namespace BlockEditor.Helpers
{

    public static class MapUtil
    {

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

        public static readonly SKPaint SelectionFillPaint = new SKPaint
        {
            Color = new SKColor(100, 200, 255, 50),
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint SelectionStrokePaint = new SKPaint
        {
            Color = new SKColor(100, 200, 255),
            Style = SKPaintStyle.Stroke,
        };

        public static readonly SKPaint TranslucentPaint = new SKPaint 
        { 
            Color = SKColors.White.WithAlpha(127) 
        };

        public static void MoveRelativeArt(IEnumerable<Art> art, int x, int y)
        {
            if (art == null)
                return;

            if (art.Count() > 0)
            {
                art.First().X += x;
                art.First().Y += y;
            }
        }

        public static void MoveAbsoluteArt(IEnumerable<Art> art, int x, int y)
        {
            if (art == null)
                return;

            foreach (Art a in art)
            {
                a.X += x;
                a.Y += y;
            }
        }

        public static void ChangeArtColor(IEnumerable<Art> art, SKColor? replace, SKColor? add, ColorSensitivty sensitivity, bool hex)
        {
            if (art == null)
                return;

            foreach (Art a in art)
            {
                var color  = ColorUtil.ToSkColor(hex ? ColorUtil.GetColorFromHex(a.Color) : ColorUtil.GetColorFromBlockOption(a.Color));

                if (!ColorUtil.IsColorEqual(color, replace, sensitivity))
                    continue;

                a.Color = hex ? ColorUtil.ToHexString(add.Value) : ColorUtil.ToIntString(add.Value);
            }
        }

    }


    public static class SaveMapUtil
    {
        public static void Save(Map map)
        {
            if (map == null)
                return;

            var save = new SaveMapWindow(map);
            var success = save.ShowDialog();

            if (success != true)
                return;

            if (!string.IsNullOrWhiteSpace(save.LocalFilepath))
            {
                SaveLocalFile(save.LocalFilepath, map);
            }
            else
            {
                map.Level.Title = save.MapTitle;
                App.MyMainWindow?.TitleChanged(map.Level.Title);
                Upload(map, save.Publish, save.Newest);
            }
        }


        private static void Upload(Map map, bool publish, bool newest)
        {
            using (new TempCursor(Cursors.Wait))
            {
                try
                {
                    if (!Users.IsLoggedIn())
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

                    if (string.Equals("m3", map.Level.DataVersion, StringComparison.InvariantCultureIgnoreCase))
                        map.Level.DataVersion = "m4";

                    var data = map.ToPr2String(Users.Current.Name, Users.Current.Token, publish, false, newest);

                    var msg = DataAccess.PR2Accessor.Upload(data, (arg) =>
                    {
                        AskToOverwrite(arg);

                        if (arg.TryAgain)
                            arg.NewLevelData = map.ToPr2String(Users.Current.Name, Users.Current.Token, publish, true, newest);
                    });

                    if (msg != null && msg.Contains("message=", StringComparison.InvariantCultureIgnoreCase))
                        msg = msg.Replace("message=", "");

                    MessageUtil.ShowInfo(msg);
                }
                catch (Exception ex)
                {
                    if (!MyUtil.HasInternet())
                        MessageUtil.ShowError("Failed to save level, check ur internet connection...");
                    else
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
                    App.MyMainWindow?.TitleChanged(map.Level.Title);

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

    }
}
