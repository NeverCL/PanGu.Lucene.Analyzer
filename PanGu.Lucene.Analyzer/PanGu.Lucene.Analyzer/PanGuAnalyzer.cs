using PanGu.Lucene.Analyzer;
using System.IO;

namespace Lucene.Net.Analysis
{
    public class PanGuAnalyzer : Analyzer
    {
        private readonly bool _originalResult;

        public PanGuAnalyzer()
        {
        }

        /// <summary>
        /// Return original string.
        /// Does not use only segment
        /// </summary>
        /// <param name="originalResult"></param>
        public PanGuAnalyzer(bool originalResult)
        {
            _originalResult = originalResult;
        }


        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseFilter(new PanGuTokenizer(reader, _originalResult));
        }
    }

}
