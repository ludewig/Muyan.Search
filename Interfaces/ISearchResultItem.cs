using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public interface ISearchResultItem
    {
        float Score { get; set; }
    }
}
