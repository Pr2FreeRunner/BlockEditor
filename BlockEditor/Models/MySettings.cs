using BlockEditor.Helpers;
using System;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{
    public static class MySettings
    {

        private static string _username;
        public static string Username
        {
            get { return _username; }
            set { _username = value; Save(); }
        }

        private static string _token;
        public static string Token
        {
            get { return _token; }
            set { _token = value; Save(); }
        }

        private static bool _firstTimeLoad;
        public static bool FirstTimeLoad
        {
            get { return _firstTimeLoad; }
            set { _firstTimeLoad = value; Save(); }
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

        public static void Init()
        {
            Load();
        }

        private static void Save()
        {
            try
            {
                Settings.Default["Username"] = Username;
                Settings.Default["Token"] = Token;
                Settings.Default["Pr2BuildVersion"] = Pr2BuildVersion;
                Settings.Default["FirstTimeLoad"] = FirstTimeLoad;
                Settings.Default["Zoom"] = (int)Zoom;

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
                _username = Settings.Default["Username"] as string ?? string.Empty;
                _token = Settings.Default["Token"] as string ?? string.Empty;
                _firstTimeLoad = (bool) Settings.Default["FirstTimeLoad"];
                _zoom = (BlockSize)(Settings.Default["Zoom"] ?? BlockImages.DEFAULT_BLOCK_SIZE);
                _pr2BuildVersion = HandleBuildVersion(Settings.Default["Pr2BuildVersion"] as string);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

        }
    }
}
