using System;
using DataAccess;
using BlockEditor.Helpers;
using BlockEditor.Views;

namespace BlockEditor.Models
{
    public static class CurrentUser
    {
        public static string Name { get; set; }

        public static string Token { get; set; }


        public static bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(Token)
                && !string.IsNullOrWhiteSpace(Name);
        }

        public static void Logout()
        {
            Token = string.Empty;
            Name = string.Empty;
        }

        private static string GetBuildVersion()
        {
            var buildVersion = "6-jul-2022-v167_1";
            var text = "What is the current Build-Version for Pr2?" 
                + Environment.NewLine 
                + Environment.NewLine 
                + "Note: This can be found on the bottom right of the Credit page.";
            
            return UserInputWindow.Show(text, "Input", buildVersion);
        }

        public static bool Login(string username, string password, out string errorMsg)
        {
            var fallbackError = "Failed to login";
            Logout();

            try
            {
                var version   = GetBuildVersion();
                var tokenInfo = PR2Accessor.GetToken(username, password, version);

                if (tokenInfo.Success)
                {
                    errorMsg = string.Empty;
                    Name = username;
                    Token = tokenInfo.Token;
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
