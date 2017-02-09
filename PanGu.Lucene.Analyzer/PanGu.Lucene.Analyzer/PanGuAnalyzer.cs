using PanGu.Lucene.Analyzer;
using System.IO;
using PanGu.Match;

namespace Lucene.Net.Analysis
{
    public class PanGuAnalyzer : Analyzer
    {
        private readonly bool _originalResult;
        /// <summary>
        /// 分词选项
        /// </summary>
        public MatchOptions Options { get; set; }

        /// <summary>
        /// 分词参数
        /// </summary>
        public MatchParameter MatchParameter { get; set; }

        public PanGuAnalyzer()
        {
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">分词选项</param>
        /// <param name="parameters">分词参数</param>
        /// <param name="originalResult">为true不分词,为false分词</param>
        public PanGuAnalyzer(MatchOptions options, MatchParameter parameters, bool originalResult = false)
        {
            Options = options;
            MatchParameter = parameters;
            _originalResult = originalResult;
        }


        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseFilter(new PanGuTokenizer(reader, Options, MatchParameter, _originalResult));
        }
    }

}
