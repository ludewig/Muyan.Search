using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;

namespace Muyan.Search
{
    public class JieBaTokenizer : Tokenizer
    {
        private System.Collections.Generic.List<JiebaNet.Segmenter.Token> _wordList = new List<JiebaNet.Segmenter.Token>();
        private string _inputText;

        private ICharTermAttribute _termAtt;
        private IOffsetAttribute _offsetAtt;
        private IPositionIncrementAttribute _posIncrAtt;
        private ITypeAttribute _typeAtt;
        private Dictionary<string,int> _stopWords = new Dictionary<string, int>();

        private IEnumerator<JiebaNet.Segmenter.Token> _iter;
        private readonly JiebaSegmenter _segmenter;
        private readonly TokenizerMode _mode;

        public JieBaTokenizer(TextReader input, TokenizerMode mode)
            : base(AttributeFactory.DEFAULT_ATTRIBUTE_FACTORY, input)
        {
            _segmenter = new JiebaSegmenter();
            _mode = mode;
            //LoadStopWords(stopUrl);
            Init();

        }
        /// <summary>
        /// 加载停用词
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadStopWords(string filePath)
        {
            using (StreamReader reader=File.OpenText(AppDomain.CurrentDomain.BaseDirectory+filePath))
            {
                string tmp;
                while ((tmp=reader.ReadLine())!=null)
                {
                    if (string.IsNullOrEmpty(tmp))
                    {
                        continue;
                    }

                    if (_stopWords.ContainsKey(tmp))
                    {
                        continue;
                    }
                    _stopWords.Add(tmp,1);
                }
            }
        }

        /// <summary>
        /// 初始化（添加属性）
        /// </summary>
        private void Init()
        {
            _termAtt = AddAttribute<ICharTermAttribute>();
            _offsetAtt = AddAttribute<IOffsetAttribute>();
            _posIncrAtt = AddAttribute<IPositionIncrementAttribute>();
            _typeAtt = AddAttribute<ITypeAttribute>();
        }

        private string ReadToEnd(TextReader input)
        {
            return input.ReadToEnd();
        }

        public sealed override Boolean IncrementToken()
        {
            ClearAttributes();

            Lucene.Net.Analysis.Token token = Next();
            if (token != null)
            {
                var buffer = token.ToString();
                _termAtt.SetEmpty().Append(buffer);
                _offsetAtt.SetOffset(CorrectOffset(token.StartOffset), CorrectOffset(token.EndOffset));
                _typeAtt.Type = token.Type;
                return true;
            }
            End();
            this.Dispose();
            return false;

        }

        public Lucene.Net.Analysis.Token Next()
        {

            bool res = _iter.MoveNext();
            if (res)
            {
                JiebaNet.Segmenter.Token current = _iter.Current;
                if (current!=null)
                {
                    Lucene.Net.Analysis.Token token = new Lucene.Net.Analysis.Token(current.Word, current.StartIndex, current.EndIndex);
                    return token;
                }
                else
                {
                    return null;
                }

            }
            else
                return null;

        }

        public override void Reset()
        {
            base.Reset();
            _inputText = ReadToEnd(base.m_input);
            IEnumerable<JiebaNet.Segmenter.Token> tokens = _segmenter.Tokenize(_inputText, _mode);//获取JieBa分词Token
            _wordList.Clear();//清除分词列表
            foreach (var token in tokens)
            {
                _wordList.Add(token);
            }
            _iter = _wordList.GetEnumerator();

        }

    }


}
