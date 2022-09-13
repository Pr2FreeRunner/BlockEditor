﻿using System;
using DataAccess;
using BlockEditor.Helpers;
using BlockEditor.Views.Windows;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Net;

namespace BlockEditor.Models
{
    public class User
    {
        public string Name { get; set; }

        public string Token { get; set; }

        private const string Separator = "#752814FGH9249#";

        public override string ToString()
        {
           if(!IsValid())
                return string.Empty;

            return Name + Separator + Token;
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;

            if (string.IsNullOrWhiteSpace(Token))
                return false;

            return true;
        }

        public static User Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var split = input.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if(split == null || split.Length != 2)
                return null;

            return new User { Name = split[0], Token = split[1] };
        }
    }

    public static class Users
    {

        public static User Current { get; set; }

        public static List<User> AllUsers { get; } = new List<User>();

        private const string Separator = "#54658AF48193#";

        public static void LoadUsers(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return;

            var split = input.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            foreach(var s in split)
            {
                var u = User.Parse(s);

                if(u != null)
                    AllUsers.Add(u);
            }

            if(AllUsers.Any())
                Current = AllUsers.First();
        }

        public static void Add(string name, string token)
        {
            var u = new User { Name = name, Token = token };

            if(u.IsValid())
            {
                Current = u;
                Remove(u);
                AllUsers.Insert(0, u);
                MySettings.Save();
            }
        }

        public static void Remove(User u)
        {
            if(u == null)
                return;

            if(!u.IsValid())
                return;

            AllUsers.RemoveAll(x => string.Equals(x.Name, u.Name, StringComparison.InvariantCultureIgnoreCase));
            MySettings.Save();
        }

        public static string SaveUser()
        {
            var builder = new StringBuilder();

            foreach (var u in AllUsers)
            {
                if(u == null || !u.IsValid())
                    continue;

                builder.Append(Separator + u.ToString());
            }

            return builder.ToString();
        }

        public static bool IsLoggedIn()
        {
            if(Current == null)
                return false;

            return Users.Current.IsValid();
        }

        public static void Logout()
        {
            Current = null;
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
                    Add(username, tokenInfo.Token);
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
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    MessageUtil.ShowError("Failed login, check ur internet connection...");
                else
                    MessageUtil.ShowError(ex.Message);

                errorMsg = fallbackError;
                return false;
            }
        }
    }
}
