using BlockEditor.Utils;
using System;
using System.Globalization;
using System.Security.Policy;
using System.Text;
using static DataAccess.DataStructures.SearchLevelInfo;

namespace BlockEditor.Models
{
    public class SearchResult
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public string CreatedBy { get; set; }

        public int? PlayCount { get; set; }

        private double? _rating;
        public double? Rating
        {
            get
            {
                if (_rating == null)
                    return null;

                if (_rating.Value >= 100)
                    return _rating.Value / 100.0;

                if (_rating.Value >= 10)
                    return _rating.Value / 10.0;

                return _rating.Value;
            }

            set
            {
                _rating = value;
            }
        }


        public SearchResult(int id, string title, string createdBy, int? playCount, double? rating)
        {
            ID = id;
            Title = title;
            CreatedBy = createdBy;
            PlayCount = playCount;
            Rating = rating;
        }

        public static readonly SearchResult SLOW_DOWN = new SearchResult(int.MinValue, string.Empty, string.Empty, null, null);


        public string GetToolTip()
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(CreatedBy))
                builder.Append("Created by:  " + CreatedBy);

            if (PlayCount != null)
                builder.Append(Environment.NewLine + "Play Count:  " + PlayCount);

            if (Rating != null)
                builder.Append(Environment.NewLine + "Ratings:  " + Math.Round(Rating.Value, 2).ToString("N2", CultureInfo.InvariantCulture));

            return builder.ToString();

        }
    }


    public class MySearch
    {
        public enum SearchBy { Username, Title, ID, LocalFile, Newest, BestWeek, GetLastSearch, MyLevels }

        public SearchBy SearchType { get; set; }

        public string SearchValue { get; set; }
        public int Page { get; set; }
        public SearchOrderEnum Order { get; set; }
        public SearchDirectionEnum Direction { get; set; }

        private const string _separator = "#9872379712#";



        public MySearch()
        {
            Load();
        }

        private void Load()
        {

            try
            {
                var data = MySettings.LastSearch;

                if (string.IsNullOrWhiteSpace(data))
                    return;

                var split = data.Split(_separator);

                if (split.Length != 5)
                    return;

                if (MyUtil.TryParse(split[0], out var r0))
                    SearchType = (SearchBy)r0;

                SearchValue = split[1];

                if (MyUtil.TryParse(split[2], out var r2))
                    Page = r2;

                if (MyUtil.TryParse(split[3], out var r3))
                    Order = (SearchOrderEnum)r3;

                if (MyUtil.TryParse(split[4], out var r4))
                    Direction = (SearchDirectionEnum)r4;
            }
            catch { } 
        }

        public bool IsValid()
        {
            switch (SearchType)
            {
                case SearchBy.Username:
                case SearchBy.Title:
                case SearchBy.ID:
                    return !string.IsNullOrWhiteSpace(SearchValue) && Page > 0;

                case SearchBy.Newest:
                case SearchBy.BestWeek:
                    return Page > 0;

                case SearchBy.MyLevels:
                case SearchBy.LocalFile:
                    return false;
            }

            return false;
        }

        public void Save()
        {

            try
            {
                var builder = new StringBuilder();

                builder.Append((int)SearchType);
                builder.Append(_separator);
                builder.Append(SearchValue ?? string.Empty);
                builder.Append(_separator);
                builder.Append(Page);
                builder.Append(_separator);
                builder.Append((int)Order);
                builder.Append(_separator);
                builder.Append((int)Direction);

                MySettings.LastSearch = builder.ToString();
            }
            catch { }
        }

    }
}
