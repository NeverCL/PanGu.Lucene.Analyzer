using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PanGu.Match;

namespace PanGu.Lucene.Analyzer
{
    public class PanGuTokenizer : Tokenizer
    {
        private readonly bool _originalResult;
        private static readonly object LockObj = new object();
        private static bool _inited;
        private string _inputText;
        private readonly WordInfo[] _wordList;
        private int _position = -1; // 词汇在缓冲中的位置.


        // this tokenizer generates three attributes:
        // _termAtt, offset, and type
        private ITermAttribute _termAtt;
        private IOffsetAttribute _offsetAtt;
        private IPositionIncrementAttribute posIncrAtt;// 暂未实现
        private ITypeAttribute _typeAtt;

        private static void InitPanGuSegment()
        {
            //Init PanGu Segment.
            if (!_inited)
            {
                Segment.Init();
                _inited = true;
            }
        }

        void Init()
        {
            InitPanGuSegment();
            _termAtt = AddAttribute<ITermAttribute>();
            _offsetAtt = AddAttribute<IOffsetAttribute>();
            posIncrAtt = AddAttribute<IPositionIncrementAttribute>();
            _typeAtt = AddAttribute<ITypeAttribute>();
        }

        public PanGuTokenizer(TextReader input, MatchOptions options, MatchParameter parameters, bool originalResult) : this(input, options, parameters)
        {
            _originalResult = originalResult;
        }

        public PanGuTokenizer(TextReader input, MatchOptions options, MatchParameter parameters)
            : base(input)
        {
            lock (LockObj)
            {
                Init();
            }

            _inputText = base.input.ReadToEnd();

            if (string.IsNullOrEmpty(_inputText))
            {
                char[] readBuf = new char[1024];
                int relCount = base.input.Read(readBuf, 0, readBuf.Length);
                StringBuilder inputStr = new StringBuilder(readBuf.Length);

                while (relCount > 0)
                {
                    inputStr.Append(readBuf, 0, relCount);
                    relCount = input.Read(readBuf, 0, readBuf.Length);
                }

                if (inputStr.Length > 0)
                {
                    _inputText = inputStr.ToString();
                }
            }

            if (string.IsNullOrEmpty(_inputText))
            {
                _wordList = new WordInfo[0];
            }
            else
            {
                var segment = new Segment();
                ICollection<WordInfo> wordInfos = segment.DoSegment(_inputText, options, parameters);
                _wordList = wordInfos.ToArray();
                //_wordList = new WordInfo[wordInfos.Count];
                //wordInfos.CopyTo(_wordList, 0);
            }
        }

        public Token Next()
        {
            if (_originalResult)
            {
                string retStr = _inputText;
                _inputText = null;
                return retStr == null ? null : new Token(retStr, 0, retStr.Length);
            }

            while (true)
            {
                _position++;
                if (_position < _wordList.Length)
                {
                    if (_wordList[_position] != null)
                    {
                        var length = _wordList[_position].Word.Length;
                        var start = _wordList[_position].Position;
                        return new Token(_wordList[_position].Word, start, start + length);
                    }
                }
                else
                {
                    break;
                }
            }
            _inputText = null;
            return null;
        }

        public override bool IncrementToken()
        {
            ClearAttributes();
            Token word = Next();
            if (word != null)
            {
                _termAtt.SetTermBuffer(word.Term);
                _offsetAtt.SetOffset(word.StartOffset, word.EndOffset);
                _typeAtt.Type = word.Type;
                return true;
            }
            End();
            return false;
        }
    }

}
