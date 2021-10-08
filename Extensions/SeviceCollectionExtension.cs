using System;
using System.Collections.Generic;
using System.Text;
using JiebaNet.Segmenter;
using Microsoft.Extensions.DependencyInjection;

namespace Muyan.Search
{
    public static class SeviceCollectionExtension
    {
        public static IServiceCollection AddSearchManager(this IServiceCollection services,SearchManagerConfig config)
        {
            services.AddSingleton(config);
            services.AddSingleton<Lucene.Net.Store.FSDirectory>(Lucene.Net.Store.FSDirectory.Open(config.FacetPath));
            services.AddSingleton<Lucene.Net.Store.Directory>(Lucene.Net.Store.FSDirectory.Open(config.DefaultPath));
            services.AddSingleton<Lucene.Net.Analysis.Analyzer>(new JieBaAnalyzer(TokenizerMode.Search, config.StopWords));
            services.AddTransient<ISearchManager, SearchManager>();
            return services;
        }
    }
}
