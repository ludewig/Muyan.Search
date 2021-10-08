using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Documents;

namespace Muyan.Search
{
    public class PagedSearchResult
    {
        public List<PagedSearchResultItem> Items { get; set; }
        public long Elapsed { get; set; }
        public int TotalHits { get; set; }

        public PagedSearchResult()
        {
            Items = new List<PagedSearchResultItem>();
        }
    }
}
