namespace BlockEditor.Models
{
    public class SearchResult
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public SearchResult(int id, string title)
        {
            ID = id;
            Title = title;
        }

        public static readonly SearchResult SLOW_DOWN = new SearchResult(int.MinValue, string.Empty);
    }
}
