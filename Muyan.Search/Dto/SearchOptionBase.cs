using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public class SearchOptionBase : ISearchOption
    {
        /// <summary>
        /// 最大检索量
        /// </summary>
        public int MaxHits { get ; set; }
    }
}
