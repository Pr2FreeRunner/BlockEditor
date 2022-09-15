using BlockEditor.Helpers;
using LevelModel.Models.Components;
using System;
using System.Windows.Input;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{
    public static class MySettings
    {

        private static bool _firstTimeLoad;
        public static bool FirstTimeLoad
        {
            get { return _firstTimeLoad; }
            set { _firstTimeLoad = value; Save(); }
        }

        private static bool _fillShape;
        public static bool FillShape
        {
            get { return _fillShape; }
            set { _fillShape = value; Save(); }
        }

        private static int _hotkey0;
        public static int Hotkey0
        {
            get { return _hotkey0; }
            set { _hotkey0 = value; Save(); }
        }

        private static int _hotkey1;
        public static int Hotkey1
        {
            get { return _hotkey1; }
            set { _hotkey1 = value; Save(); }
        }

        private static int _hotkey2;
        public static int Hotkey2
        {
            get { return _hotkey2; }
            set { _hotkey2 = value; Save(); }
        }

        private static int _hotkey3;
        public static int Hotkey3
        {
            get { return _hotkey3; }
            set { _hotkey3 = value; Save(); }
        }

        private static int _hotkey4;
        public static int Hotkey4
        {
            get { return _hotkey4; }
            set { _hotkey4 = value; Save(); }
        }


        private static int _hotkey5;
        public static int Hotkey5
        {
            get { return _hotkey5; }
            set { _hotkey5 = value; Save(); }
        }

        private static int _hotkey6;
        public static int Hotkey6
        {
            get { return _hotkey6; }
            set { _hotkey6 = value; Save(); }
        }

        private static int _hotkey7;
        public static int Hotkey7
        {
            get { return _hotkey7; }
            set { _hotkey7 = value; Save(); }
        }

        private static int _hotkey8;
        public static int Hotkey8
        {
            get { return _hotkey8; }
            set { _hotkey8 = value; Save(); }
        }

        private static int _hotkey9;
        public static int Hotkey9
        {
            get { return _hotkey9; }
            set { _hotkey9 = value; Save(); }
        }



        private static int _playTime;
        public static int PlayTime
        {
            get { return _playTime; }
            set { _playTime = value; Save(); }
        }


        public static string BlockEditorVersion
        {
            get { return "4.4"; }
        }


        private static BlockSize _zoom;
        public static BlockSize Zoom
        {
            get { return _zoom; }
            set { _zoom = value; Save(); }
        }


        private static string _pr2BuildVersion;
        public static string Pr2BuildVersion
        {
            get { return _pr2BuildVersion; }
            set { _pr2BuildVersion = value; Save(); }
        }


        private static bool _firstUserSelection;
        public static bool FirstUserSelection
        {
            get { return _firstUserSelection; }
            set { _firstUserSelection = value; Save(); }
        }


        private static bool _firstConnectTeleports;
        public static bool FirstConnectTeleports
        {
            get { return _firstConnectTeleports; }
            set { _firstConnectTeleports = value; Save(); }
        }

        private static bool _overwrite;
        public static bool Overwrite
        {
            get { return _overwrite; }
            set { _overwrite = value;  } // don't save, bad performance
        }

        private static bool _firstBlockInfo;
        public static bool FirstBlockInfo
        {
            get { return _firstBlockInfo; }
            set { _firstBlockInfo = value; Save(); }
        }


        private static bool _showArt;
        public static bool ShowArt
        {
            get { return _showArt; }
            set { _showArt = value; Save(); }
        }

        public static void Init()
        {
            //Reset();
            Load();
        }

        private static void Reset()
        {
            Settings.Default.Reset();
            Settings.Default.Save();
        }

        public static void Save()
        {
            try
            {
                Settings.Default["Users"] = Users.SaveUser();
                Settings.Default["Pr2BuildVersion"] = Pr2BuildVersion;
                Settings.Default["FirstTimeLoad"] = FirstTimeLoad;
                Settings.Default["FirstConnectTeleports"] = FirstConnectTeleports;  
                Settings.Default["ShowArt"] = ShowArt;
                Settings.Default["Overwrite"] = Overwrite;
                Settings.Default["FirstUserSelection"] = FirstUserSelection;
                Settings.Default["FirstBlockInfo"] = FirstBlockInfo;
                Settings.Default["FillShape"] = FillShape;
                Settings.Default["Zoom"] = (int)Zoom;
                Settings.Default["PlayTime"] = PlayTime;

                Settings.Default["Hotkey0"] = Hotkey0;
                Settings.Default["Hotkey1"] = Hotkey1;
                Settings.Default["Hotkey2"] = Hotkey2;
                Settings.Default["Hotkey3"] = Hotkey3;
                Settings.Default["Hotkey4"] = Hotkey4;
                Settings.Default["Hotkey5"] = Hotkey5;
                Settings.Default["Hotkey6"] = Hotkey6;
                Settings.Default["Hotkey7"] = Hotkey7;
                Settings.Default["Hotkey8"] = Hotkey8;
                Settings.Default["Hotkey9"] = Hotkey9;


                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private static string HandleBuildVersion(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return "6-jul-2022-v167_1";

            return input;
        }

        private static void Load()
        {
            try
            {
                Users.LoadUsers(Settings.Default["Users"] as string);
                _firstTimeLoad = (bool) Settings.Default["FirstTimeLoad"];
                _firstUserSelection = (bool)Settings.Default["FirstUserSelection"];
                _firstConnectTeleports = (bool)Settings.Default["FirstConnectTeleports"];
                _fillShape = (bool)Settings.Default["FillShape"];
                _overwrite = (bool)Settings.Default["Overwrite"];
                _showArt = (bool)Settings.Default["ShowArt"];
                _firstBlockInfo = (bool)Settings.Default["FirstBlockInfo"];
                _zoom = (BlockSize)(Settings.Default["Zoom"] ?? BlockImages.DEFAULT_BLOCK_SIZE);
                _pr2BuildVersion = HandleBuildVersion(Settings.Default["Pr2BuildVersion"] as string);
                _playTime = (int)(Settings.Default["PlayTime"] ?? 0);

                _hotkey0 = (int)(Settings.Default["Hotkey0"] ?? Block.BASIC_WHITE);
                _hotkey1 = (int)(Settings.Default["Hotkey1"] ?? Block.BASIC_WHITE);
                _hotkey2 = (int)(Settings.Default["Hotkey2"] ?? Block.BASIC_WHITE);
                _hotkey3 = (int)(Settings.Default["Hotkey3"] ?? Block.BASIC_WHITE);
                _hotkey4 = (int)(Settings.Default["Hotkey4"] ?? Block.BASIC_WHITE);
                _hotkey5 = (int)(Settings.Default["Hotkey5"] ?? Block.BASIC_WHITE);
                _hotkey6 = (int)(Settings.Default["Hotkey6"] ?? Block.BASIC_WHITE);
                _hotkey7 = (int)(Settings.Default["Hotkey7"] ?? Block.BASIC_WHITE);
                _hotkey8 = (int)(Settings.Default["Hotkey8"] ?? Block.BASIC_WHITE);
                _hotkey9 = (int)(Settings.Default["Hotkey9"] ?? Block.BASIC_WHITE);


            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

        }


        public static int? GetBlockId(Key k)
        {
            switch (k)
            {
                case Key.D0: case Key.NumPad0: return MySettings.Hotkey0;
                case Key.D1: case Key.NumPad1: return MySettings.Hotkey1;
                case Key.D2: case Key.NumPad2: return MySettings.Hotkey2;
                case Key.D3: case Key.NumPad3: return MySettings.Hotkey3;
                case Key.D4: case Key.NumPad4: return MySettings.Hotkey4;
                case Key.D5: case Key.NumPad5: return MySettings.Hotkey5;
                case Key.D6: case Key.NumPad6: return MySettings.Hotkey6;
                case Key.D7: case Key.NumPad7: return MySettings.Hotkey7;
                case Key.D8: case Key.NumPad8: return MySettings.Hotkey8;
                case Key.D9: case Key.NumPad9: return MySettings.Hotkey9;

                default: return null;
            }
        }

        public static void SetBlockId(Key k, int id)
        {
            switch (k)
            {
                case Key.D0: case Key.NumPad0: MySettings.Hotkey0 = id; break;
                case Key.D1: case Key.NumPad1: MySettings.Hotkey1 = id; break;
                case Key.D2: case Key.NumPad2: MySettings.Hotkey2 = id; break;
                case Key.D3: case Key.NumPad3: MySettings.Hotkey3 = id; break;
                case Key.D4: case Key.NumPad4: MySettings.Hotkey4 = id; break;
                case Key.D5: case Key.NumPad5: MySettings.Hotkey5 = id; break;
                case Key.D6: case Key.NumPad6: MySettings.Hotkey6 = id; break;
                case Key.D7: case Key.NumPad7: MySettings.Hotkey7 = id; break;
                case Key.D8: case Key.NumPad8: MySettings.Hotkey8 = id; break;
                case Key.D9: case Key.NumPad9: MySettings.Hotkey9 = id; break;
            }
        }
    }
}
