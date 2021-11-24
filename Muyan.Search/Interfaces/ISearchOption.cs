using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public interface ISearchOption
    {
        /// <summary>
        /// 最大检索量
        /// </summary>
        int MaxHits { get; set; }

    }
}
