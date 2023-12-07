namespace Lab5_Lexical_Analyzer.Enums
{
    public enum States
    {
        Start,
        Final,
        Const,
        Delimiter,
        Word,
        Arithmetic,
        Assignment,
        ComparisonRight,
        ComparisonLeft,
        Comparison,
        Error
    }
}