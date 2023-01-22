using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using BlockEditor.Models;
using LevelModel.Models.Components.Art;

namespace BlockEditor.Helpers
{
    public static class TasUtil
    {
        private static readonly List<Process> _processes = new List<Process>();


        private static void StartProcess(string levelFilepath)
        {
            var exePath = Path.Combine(Directory.GetCurrentDirectory(), "Dependencies", "TAS", "TAS.exe");
            var proc = new Process();

            proc.StartInfo.FileName = exePath;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            proc.StartInfo.Arguments = "\"" + levelFilepath + "\"" + " " + true.ToString(CultureInfo.InvariantCulture);

            KillRunningTasProcess();
            proc.Start();
            _processes.Add(proc);
        }

        private static void KillRunningTasProcess()
        {
            if (!MySettings.MaxOneTasRunning)
                return;

            foreach (var p in _processes)
            {
                try
                {
                    if (p == null)
                        continue;

                    if (!IsRunning(p.ProcessName))
                        continue;

                    p.Kill();
                }
                catch { } // ignore
            }
        }

        public static bool IsRunning(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return false;

            return Process.GetProcessesByName(name).Length > 0;
        }

        public static void Start(Map map)
        {
            if (map == null)
                return;

            var noteText = map.Level.Note;
            var textArt1 = map.Level.TextArt1;
            var textArt2 = map.Level.TextArt2;
            var textArt3 = map.Level.TextArt3;
            var textArt0 = map.Level.TextArt0;
            var textArt00 = map.Level.TextArt00;

            try
            {
                map.Level.Note = string.Empty;
                map.Level.TextArt1 = new List<TextArt>();
                map.Level.TextArt2 = new List<TextArt>();
                map.Level.TextArt3 = new List<TextArt>();
                map.Level.TextArt0 = new List<TextArt>();
                map.Level.TextArt00 = new List<TextArt>();

                var content = map.ToPr2String(string.Empty, string.Empty, false, false, false);
                var filepath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";

                File.WriteAllText(filepath, content);

                StartProcess(filepath);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
            finally
            {
                map.Level.Note = noteText;
                map.Level.TextArt1 = textArt1;
                map.Level.TextArt2 = textArt2;
                map.Level.TextArt3 = textArt3;
                map.Level.TextArt0 = textArt0;
                map.Level.TextArt00 = textArt00;
            }
        }
    }
}
