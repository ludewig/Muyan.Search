using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Store;

namespace Muyan.Search
{
    public interface ISearchManager<TAnalyzer,TDirectory, TTaxoDirectory> where TAnalyzer:Analyzer where TDirectory:Directory where TTaxoDirectory:FSDirectory
    {
        /// <summary>
        /// 分析器
        /// </summary>
        TAnalyzer Analyzer { get; }
        /// <summary>
        /// 普通索引存储路径
        /// </summary>
        TDirectory Directory { get; }

        /// <summary>
        /// 维度索引存储路径
        /// </summary>
        TTaxoDirectory TaxoDirectory { get; }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="fields"></param>
        void CreateIndex(Dictionary<string, string> fields);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="docId">文档</param>
        void DeleteIndex(int docId);

        /// <summary>
        /// 删除所有索引
        /// </summary>
        void DeleteAllIndex();

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="doc">文档</param>
        /// <param name="dic">要更新的域</param>
        void UpdateIndex(Document doc, Dictionary<string, string> dic);

        /// <summary>
        /// 索引计数
        /// </summary>
        /// <returns></returns>
        int CountIndex();

        /// <summary>
        /// 关键词查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Dictionary<Document, float> SearchIndex(string keyword, string field);

        /// <summary>
        /// 为实体对象创建索引
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="isFiltered">是否启用属性过滤，默认开启</param>
        /// <param name="isCreate">新增或更新，默认新增</param>
        void CreateIndexByEntity(IEntity<string> entity, bool isFiltered=true,bool isCreate=true);


        /// <summary>
        /// 简单查询（多域）
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        SingleSearchResult SingleSearch(SingleSearchOption option);

        /// <summary>
        /// 包含分页的查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        PagedSearchResult PagedSearch(PagedSearchOption option);

        /// <summary>
        /// 包含权重的查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        ScoredSearchResult ScoredSearch(ScoredSearchOption option);

        /// <summary>
        /// 计数查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        List<CountSearchResultItem> CountSearch(CountSearchOption option);

        /// <summary>
        /// 维度查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        FacetSearchResult FacetSearch(FacetSearchOption option);
    }
    public interface ISearchManager:ISearchManager<Analyzer,Directory,FSDirectory>
    {

    }
}
