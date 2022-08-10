using System;
using DataAccess;
using BlockEditor.Helpers;
using BlockEditor.Views;
using BlockEditor.Views.Windows;

namespace BlockEditor.Models
{
    public static class CurrentUser
    {

        public static string Name { 
            get { return MySettings.Username; }
            set { MySettings.Username = value; }
        }

        public static string Token
        {
            get { return MySettings.Token; }
            set { MySettings.Token = value; }
        }

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(Token) && !string.IsNullOrWhiteSpace(Name);
        }

        public static void Logout()
        {
            Token = string.Empty;
            Name = string.Empty;
        }

        private static string GetBuildVersion()
        {
            var buildVersion = MySettings.Pr2BuildVersion;
            var text = "What is the current build version for PR2?" 
                + Environment.NewLine 
                + Environment.NewLine 
                + "Note: This can be found on the bottom right of the Credit page.";
            
            return UserInputWindow.Show(text, "Input", buildVersion);
        }

        public static bool Login(string username, string password, out string errorMsg)
        {
            var fallbackError = "Failed to login.";
            Logout();

            try
            {
                var version = GetBuildVersion();

                if(string.IsNullOrWhiteSpace(version))
                {
                    errorMsg = string.Empty;
                    return false;
                }

                var tokenInfo = PR2Accessor.GetToken(username, password, version);

                if (tokenInfo.Success)
                {
                    errorMsg = string.Empty;
                    Name = username;
                    Token = tokenInfo.Token;
                    MySettings.Pr2BuildVersion = version;
                }
                else
                {
                    errorMsg = string.IsNullOrWhiteSpace(tokenInfo.ErrorMsg) ? fallbackError : tokenInfo.ErrorMsg;
                }

                return tokenInfo.Success;
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
                errorMsg = fallbackError;
                return false;
            }
        }
    }
}
