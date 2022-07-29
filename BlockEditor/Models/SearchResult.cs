namespace BlockEditor.Models
{
    public class SearchResult
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public string CreatedBy { get; set; }

        public SearchResult(int id, string title, string createdBy)
        {
            ID = id;
            Title = title;
            CreatedBy = createdBy;
        }

        public static readonly SearchResult SLOW_DOWN = new SearchResult(int.MinValue, string.Empty, string.Empty);
    }
}
