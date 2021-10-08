using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Facet;

namespace Muyan.Search
{
    public class FacetSearchResult
    {
        public virtual IList<FacetResult> Items { get; set; }

        public FacetSearchResult()
        {
            Items = new List<FacetResult>();
        }
    }
}
