using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public class ScoredSearchResult : ISearchResult<SearchResultItem>
    {
        public List<SearchResultItem> Items { get; set; }
        public long Elapsed { get;set;}
        public int TotalHits { get; set; }

        public ScoredSearchResult()
        {
            Items = new List<SearchResultItem>();
        }
    }
}
