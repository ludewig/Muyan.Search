using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public interface ISearchResult:ISearchResult<ISearchResultItem>
    {

    }

    public interface ISearchResult<T> where T: ISearchResultItem
    {
        List<T> Items { get; set; }
        long Elapsed { get; set; }
        int TotalHits { get; set; }
    }
}
