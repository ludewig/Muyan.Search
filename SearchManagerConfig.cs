using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public class SearchManagerConfig
    {
        /// <summary>
        /// 默认索引存储路径
        /// </summary>
        public virtual string DefaultPath { get; set; }
        /// <summary>
        /// 维度索引存储路径
        /// </summary>
        public virtual string FacetPath { get; set; }
        /// <summary>
        /// 停用词路径
        /// </summary>
        public virtual string StopWords { get; set; }
    }
}
