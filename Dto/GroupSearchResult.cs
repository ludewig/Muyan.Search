using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public class GroupSearchResult:ISearchResult<string>
    {
        public virtual IList<string> Items { get; set; }
        public virtual long Elapsed { get ; set; }
        public virtual int TotalHits { get ; set ; }

        public GroupSearchResult()
        {
            Items = new List<string>();
            Elapsed = 0;
            TotalHits = 0;
        }
    }
}
