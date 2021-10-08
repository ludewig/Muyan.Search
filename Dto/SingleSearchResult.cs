using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public class SingleSearchResult : ISearchResult<SearchResultItem>
    {
        /// <summary>
        /// 匹配结果
        /// </summary>
        public List<SearchResultItem> Items { get; set; }
        /// <summary>
        /// 检索耗时
        /// </summary>
        public long Elapsed { get; set; }
        /// <summary>
        /// 匹配结果数
        /// </summary>
        public int TotalHits { get; set; }

        public SingleSearchResult()
        {
            Items = new List<SearchResultItem>();
        }
    }
}
