using System.Web;

namespace Delete.Api.Request
{
    public struct SearchOptions
    {
        public User? Author { get; set; }
        public string? Content { get; set; }
        public int? MinID { get; set; }
        public int? MaxID { get; set; }
        public string? Has { get; set; }
        public bool? Pinned { get; set; }
        public string SortBy { get; set; } = "timestamp";
        public string SortOrder { get; set; } = "desc";
        public SearchOptions(User? author, string? content, int? minID, int? maxID, string? has, bool? pinned, string sortBy = "timestamp", string sortOrder = "desc")
        {
            Author = author;
            Content = content;
            MinID = minID;
            MaxID = maxID;
            Has = has;
            Pinned = pinned;
            SortBy = sortBy;
            SortOrder = sortOrder;
        }
        public override string ToString()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["sort_by"] = SortBy;
            query["sort_order"] = SortOrder;

            if (Author != null)
            {
                query["author_id"] = Author.ID;
            }

            if (Content != null)
            {
                query["content"] = Content;
            }

            if (MinID != null)
            {
                query["min_id"] = MinID.ToString();
            }

            if (MaxID != null)
            {
                query["max_id"] = MaxID.ToString();
            }

            if (Has != null)
            {
                query["has"] = Has;
            }

            if (Pinned != null)
            {
                query["pinned"] = Pinned.ToString();
            }

            return query.ToString();
        }
    }
}
