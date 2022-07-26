using BlockEditor.Models;
using BlockEditor.Views;
using DataAccess.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace BlockEditor.Helpers
{
    public static class MapUtil
    {


        public static void Save(Map map)
        {
            if (map == null)
                return;

            var title = UserInputWindow.Show("Title of the level?", "Save", map.Title ?? string.Empty);

            if (string.IsNullOrWhiteSpace(title))
                return;

            map.Title = title;

            Upload(map);
        }

        private static void Upload(Map map)
        {
            var current = Mouse.OverrideCursor;

            try
            {
                if (!CurrentUser.IsLoggedIn())
                {
                    MessageUtil.ShowError("Requires user to login.");
                    return;
                }

                Mouse.OverrideCursor = Cursors.Wait;

                var data = map.ToPr2String(CurrentUser.Name, CurrentUser.Token, false);

                var msg = DataAccess.PR2Accessor.Upload(data, (arg) =>
                {
                    AskToOverwrite(arg);

                    if (arg.TryAgain)
                        arg.NewLevelData = map.ToPr2String(CurrentUser.Name, CurrentUser.Token, true);
                });

                MessageUtil.ShowInfo(msg);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = current;
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
    }
}
