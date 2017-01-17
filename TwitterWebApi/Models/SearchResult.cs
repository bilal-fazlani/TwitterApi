using System.Collections.Generic;

namespace TwitterWebApi.Models
{
    public class SearchResult
    {
        public ulong? SinceId { get; set; }

        public ulong? LastId { get; set; }

        public int PageSize { get; set; }

        public List<Tweet> Statuses { get; set; }
    }
}