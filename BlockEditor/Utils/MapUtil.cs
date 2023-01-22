using BlockEditor.Models;
using BlockEditor.Views;
using BlockEditor.Views.Windows;
using DataAccess.DataStructures;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Linq;
using SkiaSharp;
using BlockEditor.Utils;

namespace BlockEditor.Helpers
{

    public static class MapUtil
    {

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
            Color = SKColors.White.WithAlpha(150)
        };

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
