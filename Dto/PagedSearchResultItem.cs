using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Documents;

namespace Muyan.Search
{
    public class PagedSearchResultItem : ISearchResultItem
    {
        public float Score { get; set; }

        public Document Doc { get; set; }
    }
}
