using Lab5_Lexical_Analyzer.Enums;

namespace Lab5_Lexical_Analyzer
{
    public struct Lexeme
    {
        public LexTypes LexType { get; private set; }
        public Categories LexCat { get; private set; }
        public string Value { get; private set; }

        public int LinePos { get; private set; }
        public int LexemePos { get; private set; }
        public int CharPosAbsolute { get; private set; }

        public Lexeme(LexTypes lexType, Categories lexCat, string value, int linePos, int lexemePos, int charPos)
        {
            LexType = lexType;
            LexCat = lexCat;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            LinePos = linePos;
            LexemePos = lexemePos;
            CharPosAbsolute = charPos;
        }
    }
}