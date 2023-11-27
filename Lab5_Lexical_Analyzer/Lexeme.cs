using Lab5_Lexical_Analyzer.Enums;

namespace Lab5_Lexical_Analyzer
{
    public struct Lexeme
    {
        public LexTypes LexType { get; private set; }
        public Categories LexCat { get; private set; }
        public string Value { get; private set; }

        public Lexeme(LexTypes lexType, Categories lexCat, string value)
        {
            LexType = lexType;
            LexCat = lexCat;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}