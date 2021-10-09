using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Cn;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.Common;
using Lucene.Net.Queries;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;

namespace Muyan.Search
{

    public class SearchManager<TAnalyzer,TDirectory, TTaxoDirectory> :ISearchManager<TAnalyzer,TDirectory,TTaxoDirectory>
        where TAnalyzer:Analyzer
        where TDirectory:Directory
        where TTaxoDirectory: FSDirectory
    {
        /// <summary>
        /// 分析器
        /// </summary>
        public virtual TAnalyzer Analyzer { get;  }
        /// <summary>
        /// 普通索引存储路径
        /// </summary>
        public virtual TDirectory Directory { get;  }

        /// <summary>
        /// 维度索引存储路径
        /// </summary>
        public virtual TTaxoDirectory TaxoDirectory { get;  }

        public SearchManager(TDirectory directory, TAnalyzer analyzer,TTaxoDirectory taxoDirectory)
        {
            Directory = directory;
            Analyzer = analyzer;
            TaxoDirectory = taxoDirectory;

            //一个IndexWriterConfig实例只能供一个IndexWriter实例使用，因此不能将IndexWriterConfig作为局部公共变量使用，所以下方代码错误。
            //IndexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);

        }

        #region 创建索引
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="fields">Field域集合，也可以是其他结构</param>
        public virtual void CreateIndex(Dictionary<string, string> fields)
        {
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, Analyzer);
            using (IndexWriter writer = new IndexWriter(Directory, config))
            {
                //创建文档
                Document doc = new Document();
                foreach (var field in fields)
                {
                    //创建域。此处使用了StringField域。此外还能用Int32Field、DoubleField等。
                    //StringField:将字段索引，但不会进行分词。
                    //TextField：将字段分词后进行索引
                    Field f = new TextField(field.Key, field.Value, Field.Store.YES);
                    //添加数据域至文档
                    doc.Add(f);
                }
                //索引操作器增加文档
                writer.AddDocument(doc);
                //刷新索引
                writer.Flush(true, true);
                writer.Commit();
            }
        }
        #endregion

        #region 删除所有索引
        /// <summary>
        /// 删除所有索引
        /// </summary>
        public virtual void DeleteAllIndex()
        {
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, Analyzer);
            using (IndexWriter writer = new IndexWriter(Directory, config))
            {
                IndexReader reader = DirectoryReader.Open(Directory);
                writer.DeleteAll();
                writer.Commit();
            }
        }
        #endregion

        #region 删除索引
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="docId">文档Id</param>
        public virtual void DeleteIndex(int docId)
        {
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, Analyzer);
            using (IndexWriter writer = new IndexWriter(Directory, config))
            {
                IndexReader reader = DirectoryReader.Open(Directory);
                writer.TryDeleteDocument(reader, docId);
            }
        }
        #endregion

        #region 关键词查询
        /// <summary>
        /// 关键词查询
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="field">域</param>
        /// <param name="route">路由</param>
        /// <returns></returns>
        public virtual Dictionary<Document, float> SearchIndex(string keyword, string field)
        {
            //定义返回数据结构
            Dictionary<Document, float> dic = new Dictionary<Document, float>();
            //实例化索引读取器
            using (Lucene.Net.Index.DirectoryReader reader = DirectoryReader.Open(Directory))
            {
                //实例化索引检索器
                IndexSearcher searcher = new IndexSearcher(reader);
                //创建查询生成器。此处使用了基础的查询生成器QueryPaser，构造函数参数为版本、查询的域、分析器。
                //也可以使用MultiFieldQueryPaser多域查询生成器。
                QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, field,
                    new StandardAnalyzer(LuceneVersion.LUCENE_48));
                //生成查询对象。
                Query query = parser.Parse(keyword);
                //检索并返回结果。检索条件为返回符合程度最高的前10条。
                TopDocs matches = searcher.Search(query, 10);
                //遍历检索结构，构造需要返回的数据结构
                foreach (var match in matches.ScoreDocs)
                {
                    //获取匹配结果中的文档
                    Document doc = searcher.Doc(match.Doc);
                    //获取匹配文档的分数
                    dic.Add(doc, match.Score);
                }
            }

            return dic;
        }


        #endregion

        #region 更新索引
        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="doc">文档</param>
        /// <param name="dic">要更新的域</param>
        public virtual void UpdateIndex(Document doc, Dictionary<string, string> dic)
        {
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, Analyzer);
            using (IndexWriter writer = new IndexWriter(Directory, config))
            {
                foreach (var term in dic)
                {
                    writer.UpdateDocument(new Term(term.Key, term.Value), doc);
                }
                writer.Flush(true, true);
                writer.Commit();
            }
        }
        #endregion

        #region 索引计数
        /// <summary>
        /// 索引计数
        /// </summary>
        /// <returns></returns>
        public virtual int CountIndex()
        {
            //实例化索引读取器
            using (Lucene.Net.Index.DirectoryReader reader = DirectoryReader.Open(Directory))
            {
                return reader.NumDocs;
            }
        }
        #endregion

        #region 为实体创建索引
        /// <summary>
        /// 为实体对象创建索引
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="isFiltered">是否启用属性过滤，默认开启</param>
        /// <param name="isCreate">新增或更新，默认新增</param>
        public virtual void CreateIndexByEntity(IEntity<string> entity,bool isFiltered=true,bool isCreate = true)
        {
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, Analyzer);
            if (isCreate)
            {
                config.OpenMode = OpenMode.CREATE;
            }
            else
            {
               
                config.OpenMode = OpenMode.CREATE_OR_APPEND;
            }

            using (IndexWriter writer = new IndexWriter(Directory, config))
            {
                Document doc = new Document();
                //创建文档
                var type = entity.GetType();
                //为实体所在的类名和Id创建Field，目的是对实体进行标识，便于以后检索
                //文档Document是域Field的集合，本身并无标识。通过添加Id的域对文档进行标识。
                //检索结束后可以从匹配的文档中反向找出对应的数据库实体，与业务建立关联
                doc.Add(new StringField(CoreConstant.EntityType, type.AssemblyQualifiedName, Field.Store.YES));//添加表名/类名的域
                doc.Add(new StringField(CoreConstant.EntityId, entity.Id, Field.Store.YES));//添加记录Id/标识的域
                var properties = type.GetProperties();
                //遍历实体的成员集合
                foreach (var propertyInfo in properties)
                {
                    var propertyValue = propertyInfo.GetValue(entity);
                    if (propertyValue == null)
                    {
                        continue;
                    }
                    string fieldName = propertyInfo.Name;//成员字段名称

                    if (isFiltered)
                    {
                        var attributes = propertyInfo.GetCustomAttributes<IndexAttribute>();//获取自定义属性集合
                        foreach (var attribute in attributes)
                        {
                            string name = string.IsNullOrEmpty(attribute.FieldName) ? fieldName : attribute.FieldName;

                            if (propertyValue.IsNull()||string.IsNullOrEmpty(propertyValue.ToString()))
                            {
                                continue;
                            }

                            switch (attribute.FieldType)
                            {
                                case FieldDataType.DateTime:
                                    doc.Add(new StringField(fieldName, ((DateTime)propertyValue).ToString("yyyy-MM-dd HH:mm:ss"), attribute.IsStore));
                                    break;
                                case FieldDataType.DateYear:
                                    doc.Add(new StringField(fieldName, propertyValue.ToString(), attribute.IsStore));
                                    break;
                                case FieldDataType.Int32:
                                    doc.Add(new Int32Field(fieldName, (Int32)propertyValue, attribute.IsStore));
                                    break;
                                case FieldDataType.Int64:
                                    doc.Add(new Int64Field(fieldName, (Int64)propertyValue, attribute.IsStore));
                                    break;
                                case FieldDataType.Double:
                                    doc.Add(new DoubleField(fieldName, (double)propertyValue, attribute.IsStore));
                                    break;
                                case FieldDataType.Html:
                                    doc.Add(new TextField(fieldName, propertyValue.ToString().ClearHtml(), attribute.IsStore));
                                    break;
                                case FieldDataType.Json:
                                    doc.Add(new TextField(fieldName, propertyValue.ToString().ClearJson(), attribute.IsStore));
                                    break;
                                case FieldDataType.Xml:
                                    doc.Add(new TextField(fieldName, propertyValue.ToString().ClearXml(), attribute.IsStore));
                                    break;
                                case FieldDataType.Csv:
                                    doc.Add(new TextField(fieldName, propertyValue.ToString().ClearCsv(), attribute.IsStore));
                                    break;
                                case FieldDataType.Dic:
                                    Dictionary<string, string> dic = new Dictionary<string, string>();
                                    foreach (var kv in dic)
                                    {
                                        doc.Add(new TextField(kv.Key, kv.Value, attribute.IsStore));
                                    }
                                    break;
                                case FieldDataType.Text:
                                    doc.Add(new TextField(fieldName, propertyValue.ToString(), attribute.IsStore));
                                    break;
                                case FieldDataType.Facet:
                                    doc.Add(new FacetField(fieldName, propertyValue.ToString()));
                                    break;
                                default:
                                    doc.Add(new StringField(fieldName, propertyValue.ToString(), attribute.IsStore));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (propertyValue)
                        {
                            case DateTime time:
                                doc.Add(new StringField(fieldName, time.ToString("yyyy-MM-dd HH:mm:ss"), Field.Store.YES));
                                break;
                            case int num:
                                doc.Add(new Int32Field(fieldName, num, Field.Store.YES));
                                break;
                            case long num:
                                doc.Add(new Int64Field(fieldName, num, Field.Store.YES));
                                break;
                            case double num:
                                doc.Add(new DoubleField(fieldName, num, Field.Store.YES));
                                break;
                            default:
                                doc.Add(new TextField(fieldName, propertyValue.ToString(), Field.Store.YES));
                                break;
                        }
                    }

                }

                using (DirectoryTaxonomyWriter taxonomyWriter = new DirectoryTaxonomyWriter(TaxoDirectory))
                {
                    FacetsConfig facetsConfig = new FacetsConfig();
                    doc = facetsConfig.Build(taxonomyWriter, doc);
                }
                if (writer.Config.OpenMode == OpenMode.CREATE)
                {
                    writer.AddDocument(doc);
                }
                else
                {
                    writer.UpdateDocument(new Term(CoreConstant.EntityId, entity.Id), doc);
                }
                //刷新索引
                writer.Flush(true, true);
                writer.Commit();


            }
        }



        #endregion

        #region 简单查询
        /// <summary>
        /// 简单查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public SingleSearchResult SingleSearch(SingleSearchOption option)
        {
            SingleSearchResult result = new SingleSearchResult();
            Stopwatch watch=Stopwatch.StartNew();
            using (Lucene.Net.Index.DirectoryReader reader = DirectoryReader.Open(Directory))
            {
                //实例化索引检索器
                IndexSearcher searcher = new IndexSearcher(reader);
                var queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, option.Fields.ToArray(), Analyzer);
                Query query = queryParser.Parse(option.Keyword);
                var matches = searcher.Search(query, option.MaxHits).ScoreDocs;

                #region 高亮

                QueryScorer scorer = new QueryScorer(query);
                Highlighter highlighter = new Highlighter(scorer);

                #endregion

                result.TotalHits = matches.Count();
                foreach (var match in matches)
                {
                    var doc = searcher.Doc(match.Doc);
                    SearchResultItem item = new SearchResultItem();
                    item.Score = match.Score;
                    item.EntityId = doc.GetField(CoreConstant.EntityId).GetStringValue();
                    item.EntityName = doc.GetField(CoreConstant.EntityType).GetStringValue();
                    String storedField = doc.Get(option.Fields[0]);
                    if (option.IsHightLight)//高亮
                    {
                        TokenStream stream = TokenSources.GetAnyTokenStream(reader, match.Doc, option.Fields[0], doc, Analyzer);
                        IFragmenter fragmenter = new SimpleSpanFragmenter(scorer);
                        highlighter.TextFragmenter = fragmenter;
                        string fragment = highlighter.GetBestFragment(stream, storedField);
                        item.FieldValue = fragment;
                    }
                    else
                    {
                        item.FieldValue = storedField;
                    }
                    result.Items.Add(item);
                }
            }
            watch.Stop();
            result.Elapsed = watch.ElapsedMilliseconds;
            return result;
        }


        #endregion

        #region 关键词分割
        /// <summary>
        /// 关键词分割
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private List<string> Cut(string keyword)
        {
            List<string> result = new List<string> { keyword };//先将关键词放入分割结果中
            if (keyword.Length <= 2)//如果关键词过短则不分割，直接返回结果
            {
                return result;
            }
            //常用关键词查询规则替换，‘+’替换并，‘-’替换否，空格替换或
            keyword = keyword.Replace("AND ", "+").Replace("NOT ", "-").Replace("OR ", " ");

            result.AddRange(Regex.Matches(keyword, @""".+""").Cast<Match>().Select(m =>
            {
                keyword = keyword.Replace(m.Value, "");
                return m.Value;
            }));//必须包含的
            result.AddRange(Regex.Matches(keyword, @"\s-.+\s?").Cast<Match>().Select(m =>
            {
                keyword = keyword.Replace(m.Value, "");
                return m.Value.Trim();
            }));//必须不包含的

            result.AddRange(Regex.Matches(keyword, @"[\u4e00-\u9fa5]+").Cast<Match>().Select(m => m.Value));//中文
            result.AddRange(Regex.Matches(keyword, @"\p{P}?[A-Z]*[a-z]*[\p{P}|\p{S}]*").Cast<Match>().Select(m => m.Value));//英文单词
            result.AddRange(Regex.Matches(keyword, "([A-z]+)([0-9.]+)").Cast<Match>().SelectMany(m => m.Groups.Cast<Group>().Select(g => g.Value)));//英文+数字
            //result.AddRange(new JiebaSegmenter().Cut(keyword, true));//结巴分词
            result.RemoveAll(s => s.Length < 2);
            result = result.Distinct().OrderByDescending(s => s.Length).Take(10).ToList();

            return result;
        }

        /// <summary>
        /// 关键词分割
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        private List<string> Cut(string keyword, bool isDefault)
        {
            List<string> result = new List<string>();
            using (var tokenStream=Analyzer.GetTokenStream(null,keyword))
            {
                tokenStream.Reset();
                var attributes = tokenStream.GetAttribute<Lucene.Net.Analysis.TokenAttributes.ICharTermAttribute>();
                while (tokenStream.IncrementToken())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        stringBuilder.Append(attributes.Buffer[i]);
                    }

                    string item = stringBuilder.ToString();
                    if (!result.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        #endregion

        #region 包含分页的查询
        /// <summary>
        /// 包含分页的查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public PagedSearchResult PagedSearch(PagedSearchOption option)
        {
            PagedSearchResult result = new PagedSearchResult();
            Stopwatch watch = Stopwatch.StartNew();

            using (DirectoryReader reader = DirectoryReader.Open(Directory))
            {
                IndexSearcher searcher = new IndexSearcher(reader);
                var queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, option.Fields.ToArray(), Analyzer, option.Boosts);
                var terms = Cut(option.Keyword);
                Query query = QueryExpression(queryParser, terms);
                List<SortField> sortFields = new List<SortField>();
                sortFields.Add(SortField.FIELD_SCORE);
                foreach (var sort in option.Sorts)
                {
                    sortFields.Add(new SortField(sort, SortFieldType.STRING));
                }
                Sort sorts = new Sort(sortFields.ToArray());
                Expression<Func<ScoreDoc, bool>> whereExpression = m => m.Score >= option.Score;
                var matches = searcher.Search(query, null, option.MaxHits, sorts, true, true).ScoreDocs
                    .Where(whereExpression.Compile());

                matches = matches.Skip((option.PageIndex - 1) * option.PageSize);
                matches = matches.Take(option.PageSize);

                foreach (var match in matches)
                {
                    var doc = searcher.Doc(match.Doc);
                    PagedSearchResultItem item = new PagedSearchResultItem();
                    item.Score = match.Score;
                    item.Doc = doc;
                    result.Items.Add(item);
                }

                result.TotalHits = matches.Count();
            }

            watch.Stop();
            result.Elapsed = watch.ElapsedMilliseconds;
            return result;
        }
        #endregion

        #region 包含权重的查询
        /// <summary>
        /// 包含权重的查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public ScoredSearchResult ScoredSearch(ScoredSearchOption option)
        {
            ScoredSearchResult result = new ScoredSearchResult();
            Stopwatch watch = Stopwatch.StartNew();//启动计时器

            using (DirectoryReader reader = DirectoryReader.Open(Directory))
            {
                IndexSearcher searcher = new IndexSearcher(reader);
                var queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, option.Fields.ToArray(), Analyzer, option.Boosts);
                var terms = Cut(option.Keyword);//关键词分割
                Query query = QueryExpression(queryParser, terms);//查询语句拼接扩展
                Sort sort = new Sort(SortField.FIELD_SCORE);//默认按照评分排序
                Expression<Func<ScoreDoc, bool>> whereExpression = m => m.Score >= option.Score;
                var matches = searcher.Search(query, option.Filter, option.MaxHits, sort, true, true).ScoreDocs
                    .Where(whereExpression.Compile());

                foreach (var match in matches)
                {
                    var doc = searcher.Doc(match.Doc);
                    SearchResultItem item = new SearchResultItem();
                    item.Score = match.Score;
                    item.EntityId = doc.Get(CoreConstant.EntityId);
                    item.EntityName = doc.Get(CoreConstant.EntityType);
                    result.Items.Add(item);
                }

                result.TotalHits = matches.Count();
            }

            watch.Stop();//停止计时器
            result.Elapsed = watch.ElapsedMilliseconds;
            return result;
        }
        #endregion

        #region 查询语句扩展
        /// <summary>
        /// 查询语句扩展
        /// </summary>
        /// <param name="queryParser"></param>
        /// <param name="terms"></param>
        /// <returns></returns>
        private BooleanQuery QueryExpression(MultiFieldQueryParser queryParser, List<string> terms)
        {
            BooleanQuery query = new BooleanQuery();
            foreach (var term in terms)
            {
                if (term.StartsWith("\""))
                {
                    query.Add(queryParser.Parse(term.Trim('"')), Occur.MUST);//必须匹配
                }
                else if (term.StartsWith("-"))
                {
                    query.Add(queryParser.Parse(term), Occur.MUST_NOT);//必须不匹配
                }
                else
                {
                    query.Add(queryParser.Parse(term.Replace("~", "") + "~"), Occur.SHOULD);//可以匹配
                }
            }
            return query;
        }
        #endregion

        #region 计数查询

        public virtual List<CountSearchResultItem> CountSearch(CountSearchOption option)
        {
            List<CountSearchResultItem> items = new List<CountSearchResultItem>();
            using (Lucene.Net.Index.DirectoryReader reader = DirectoryReader.Open(Directory))
            {
                //实例化索引检索器
                IndexSearcher searcher = new IndexSearcher(reader);
                Fields fields = MultiFields.GetFields(reader);
                
                var terms = fields.GetTerms(option.FieldName);
                CollectionStatistics collection= searcher.CollectionStatistics(option.FieldName);
                var termFreq = reader.GetSumTotalTermFreq(option.FieldName);
                long num = collection.SumDocFreq;
            }
            return items;
        }




        #endregion

        #region 维度查询
        /// <summary>
        /// 维度查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public FacetSearchResult FacetSearch(FacetSearchOption option)
        {
            FacetSearchResult result = new FacetSearchResult();
            using (DirectoryReader reader=DirectoryReader.Open(Directory))
            {
                DirectoryTaxonomyReader taxonomyReader = new DirectoryTaxonomyReader(TaxoDirectory);
                IndexSearcher searcher = new IndexSearcher(reader);
                FacetsCollector facetsCollector = new FacetsCollector();

                FacetsCollector.Search(searcher, new MatchAllDocsQuery(), 10, facetsCollector);
                Facets facets = new FastTaxonomyFacetCounts(taxonomyReader, new FacetsConfig(), facetsCollector);

                foreach (var field in option.Fields)
                {
                    FacetResult facetResult = facets.GetTopChildren(option.MaxHits, field);
                    result.Items.Add(facetResult);
                }
            }
            return result;
        } 
        #endregion

    }

    public class SearchManager : SearchManager<Analyzer, Directory,FSDirectory>,ISearchManager
    {

        public SearchManager(Directory directory,Analyzer analyzer,FSDirectory taxoDirectory):base(directory, analyzer,taxoDirectory)
        {

        }

        #region 创建索引
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="fields">Field域集合，也可以是其他结构</param>
        public override void CreateIndex(Dictionary<string, string> fields)
        {
             base.CreateIndex(fields);
        }
        #endregion

        #region 删除所有索引
        /// <summary>
        /// 删除所有索引
        /// </summary>
        public override void DeleteAllIndex()
        {
            base.DeleteAllIndex();
        }
        #endregion

        #region 删除索引
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="docId">文档Id</param>
        public override void DeleteIndex(int docId)
        {
            base.DeleteIndex(docId);
        }
        #endregion

        #region 关键词查询
        /// <summary>
        /// 关键词查询
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="field">域</param>
        /// <returns></returns>
        public override Dictionary<Document, float> SearchIndex(string keyword, string field)
        {
            return base.SearchIndex(keyword, field);
        }


        #endregion

        #region 更新索引
        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="doc">文档</param>
        /// <param name="dic">要更新的域</param>
        public override void UpdateIndex(Document doc, Dictionary<string, string> dic)
        {
            base.UpdateIndex(doc, dic);
        }
        #endregion

        #region 索引计数
        /// <summary>
        /// 索引计数
        /// </summary>
        /// <returns></returns>
        public override int CountIndex()
        {
            return base.CountIndex();
        }
        #endregion

    }
}
