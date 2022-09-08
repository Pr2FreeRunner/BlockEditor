using BlockEditor.Helpers;
using BlockEditor.Models;
using DataAccess;
using DataAccess.DataStructures;
using LevelModel.Models;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace BlockEditor.Utils
{
    class SearchLevelUtil
    {

        public static IEnumerable<SearchResult> SearchLocalFile(Action closeWindow, Action<Level> OnSelectedLevel)
        {
            var filepath = GetLocalFilepath();

            if (string.IsNullOrWhiteSpace(filepath))
                yield break;

            var data = File.ReadAllText(filepath);

            try
            {
                var levelInfo = PR2Parser.Level(data);

                if (levelInfo?.Messages?.Where(m => m != null).Any() == true)
                {
                    MessageUtil.ShowMessages(levelInfo.Messages);
                    closeWindow?.Invoke();
                    yield break;
                }

                if (levelInfo?.Level?.Title == null)
                {
                    MessageUtil.ShowError("Failed to parse level.");
                    closeWindow?.Invoke();
                    yield break;
                }

                OnSelectedLevel?.Invoke(levelInfo.Level);
            }
            catch
            {
                MessageUtil.ShowError("Failed to parse level.");
                closeWindow?.Invoke();
            }
        }

        public static IEnumerable<SearchResult> SearchLevel(SearchLevelInfo info)
        {
            if (string.IsNullOrWhiteSpace(info?.SearchValue))
                yield break;

            var data = PR2Accessor.Search(info);

            if (IsSlowDownResponse(data))
            {
                yield return SearchResult.SLOW_DOWN;
                yield break;
            }

            var levels = PR2Parser.SearchResult(data);

            foreach (var l in levels)
            {
                if (l == null)
                    continue;

                yield return new SearchResult(l.LevelID, l.Title, l.UserName, l.PlayCount, l.Rating);
            }
        }

        public static IEnumerable<SearchResult> SearchNewest(int page)
        {
            var data = PR2Accessor.Newest(page);

            if (IsSlowDownResponse(data))
            {
                yield return SearchResult.SLOW_DOWN;
                yield break;
            }

            var levels = PR2Parser.SearchResult(data);

            foreach (var l in levels)
            {
                if (l == null)
                    continue;

                yield return new SearchResult(l.LevelID, l.Title, l.UserName, l.PlayCount, l.Rating);
            }
        }

        public static IEnumerable<SearchResult> SearchBestWeek(int page)
        {
            var data = PR2Accessor.BestWeek(page);

            if (IsSlowDownResponse(data))
            {
                yield return SearchResult.SLOW_DOWN;
                yield break;
            }

            var levels = PR2Parser.SearchResult(data);

            foreach (var l in levels)
            {
                if (l == null)
                    continue;

                yield return new SearchResult(l.LevelID, l.Title, l.UserName, l.PlayCount, l.Rating);
            }
        }

        public static IEnumerable<SearchResult> SearchMyLevels()
        {
            if (!Users.IsLoggedIn())
            {
                MessageUtil.ShowError("Requires user to login");
                yield break;
            }

            var data = PR2Accessor.LoadMyLevels(Users.Current.Token);

            if (IsSlowDownResponse(data))
            {
                yield return SearchResult.SLOW_DOWN;
                yield break;
            }

            var levels = PR2Parser.LoadResult(data);

            foreach (var l in levels)
            {
                if (l == null)
                    continue;

                yield return new SearchResult(l.LevelID, l.Title, l.UserName, l.PlayCount, l.Rating);
            }
        }

        public static IEnumerable<SearchResult> SearchByLevelId(int id, Action closeWindow, Action<Level> OnSelectedLevel)
        {
            SearchResult result = null;

            try
            {
                var data = PR2Accessor.Download(id);

                if (string.IsNullOrWhiteSpace(data))
                    yield break;

                if (IsSlowDownResponse(data))
                {
                    result = SearchResult.SLOW_DOWN;
                }
                else
                {
                    var levelInfo = PR2Parser.Level(data);

                    if (levelInfo.Messages.Any())
                    {
                        MessageUtil.ShowMessages(levelInfo.Messages);
                        closeWindow?.Invoke();
                        yield break;
                    }

                    if (levelInfo?.Level?.Title == null)
                    {
                        MessageUtil.ShowError("Failed to parse level.");
                        closeWindow?.Invoke();
                        yield break;
                    }

                    OnSelectedLevel?.Invoke(levelInfo?.Level);
                }
            }
            catch (WebException ex)
            {
                var r = ex.Response as HttpWebResponse;

                if (r != null && r.StatusCode == HttpStatusCode.NotFound)
                    yield break;
                else
                    throw;
            }

            if (result != null)
                yield return result;
        }

        public static int? GetLevelID(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (!MyUtil.TryParse(input, out var id))
                return null;

            return id;
        }

        private static string GetLocalFilepath()
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return string.Empty;
        }

        private static bool IsSlowDownResponse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return false;

            try
            {
                var json = JObject.Parse(data);
                var success = json?.GetValue("success")?.Value<bool>() ?? false;
                var msg = json?.GetValue("error")?.Value<string>() ?? string.Empty;

                return !success && msg != null && msg.Contains("Slow down", StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    
    }
}
