using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muyan.Search
{
    public class PagedSearchOption : SearchOptionBase
    {
        public int PageSize { get ; set ; }
        public int PageIndex { get ; set ; }
        public List<string> Filters { get ; set ; }
        public List<string> Sorts { get ; set ; }


        /// <summary>
        /// 检索关键词
        /// </summary>
        public string Keyword { get; set; }

        private readonly List<string> _fields;
        /// <summary>
        /// 限定检索域
        /// </summary>
        internal List<string> Fields
        {
            get
            {
                return Filters;
            }
        }

        /// <summary>
        /// 多字段搜索时，给字段设定搜索权重
        /// </summary>
        private readonly Dictionary<string, float> _boosts;

        /// <summary>
        /// 多字段搜索时，给字段设定搜索权重
        /// </summary>
        internal Dictionary<string, float> Boosts
        {
            get
            {
                foreach (var field in Fields.Where(field => _boosts.All(x => x.Key.ToUpper() != field.ToUpper())))
                {
                    _boosts.Add(field, 2.0f);
                }

                return _boosts;
            }
        }

        /// <summary>
        /// 匹配度，0-1，数值越大结果越精确
        /// </summary>
        public float Score { get; set; } = 0.5f;


        public PagedSearchOption(string keyword, List<string> filters,List<string> sorts, int maxHits = 100, Dictionary<string, float> boosts = null,int pageSize=10,int pageIndex=1)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("搜索关键词不能为空");
            }

            Keyword = keyword;
            Filters = filters;
            Sorts = sorts;
            MaxHits = maxHits;
            if (pageSize<1)
            {
                pageSize = 1;
            }

            if (pageIndex<1)
            {
                pageIndex = 1;
            }
            PageSize = pageSize;
            PageIndex = pageIndex;
            _fields = new List<string>();
            _boosts = boosts ?? new Dictionary<string, float>();

        }

        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="field"></param>
        /// <param name="boost"></param>
        public void SetBoosts(string field, float boost)
        {
            _boosts[field] = boost;
        }

    }
}
