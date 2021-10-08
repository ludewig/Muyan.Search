using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net;
using JiebaNet;
using Lucene.Net.Analysis;
using System.IO;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.TokenAttributes;

namespace Muyan.Search
{
    public class JieBaAnalyzer : Analyzer
    {
        private readonly TokenizerMode _mode;
        private string _stopUrl;

        public JieBaAnalyzer(TokenizerMode mode,string stopUrl= "./Resources/stopwords.txt") :base()
        {
            this._mode = mode;
            _stopUrl = stopUrl;
        }
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var tokenizer = new JieBaTokenizer(reader, _mode,_stopUrl);

            var tokenstream = (TokenStream)new LowerCaseFilter(Lucene.Net.Util.LuceneVersion.LUCENE_48, (TokenStream)tokenizer);

            tokenstream.AddAttribute<ICharTermAttribute>();
            tokenstream.AddAttribute<IOffsetAttribute>();

            return new TokenStreamComponents((Tokenizer)tokenizer, tokenstream);
        }
    }
}
