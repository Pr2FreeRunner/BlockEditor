using BlockEditor.Helpers;
using BlockEditor.Views.Windows;
using LevelModel.Models.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BlockEditor.Models
{
    public static class ConnectTeleports
    {

        private static List<SimpleBlock> Blocks { get; }

        static ConnectTeleports()
        {
            Blocks = new List<SimpleBlock>();

        }

        public static void Start()
        {
            Blocks.Clear();

            if (MySettings.FirstConnectTeleports)
            {
                MessageUtil.ShowInfo("Hint:  Click on all the teleport blocks you wish to connect."
                    + Environment.NewLine + Environment.NewLine
                    + "Then click on the 'Connect Teleports' button again.");
                MySettings.FirstConnectTeleports = false;
            }
        }

        public static void Add(SimpleBlock b)
        {
            if(b.IsEmpty())
                return;

            if(b.ID != Block.TELEPORT)
                return;

            Blocks.Add(b);
        }

        public static List<SimpleBlock> End()
        {
            var result = new List<SimpleBlock>();

            if(Blocks.Count == 0)
                return result;

            ConnectTeleportsWindow w = null;
            using (new TempCursor(null)) 
            { 
                w = new ConnectTeleportsWindow(Blocks.Count);
                w.ShowDialog();
            }

            if (w.DialogResult != true)
                return result;

            foreach(var b in Blocks)
            {
                if(b.IsEmpty())
                    continue;

                result.Add(new SimpleBlock(b.ID, b.Position.Value, w.Option));
            }

            return result;
        }
    }
}
