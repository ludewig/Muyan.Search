using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public class FacetSearchOption
    {
        public virtual List<string> Fields { get; set; }

        public virtual int MaxHits { get; set; }

        public FacetSearchOption()
        {
            Fields = new List<string>();
            MaxHits = 10;
        }

        public FacetSearchOption(List<string> fields,int maxHits=10)
        {
            Fields = fields;
            MaxHits = maxHits;
        }
    }
}
