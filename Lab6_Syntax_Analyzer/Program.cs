using Lab5_Lexical_Analyzer;

namespace Lab6_Syntax_Analyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string code = @"for i = 1 + 2 to i + 23
                                a = a - 1 + 5
                                b = 56
                            next";

            (bool, List<Lexeme>) resLexemes = LexAnalyzer.Analyze(code);

            for (int i = 0; i < resLexemes.Item2.Count; i++)
            {
                Console.WriteLine($"{i}. Category: {resLexemes.Item2[i].LexCat}, Type: {resLexemes.Item2[i].LexType}, " +
                    $"Value: {resLexemes.Item2[i].Value}");
            }
            Console.WriteLine(resLexemes.Item1 ? "Success" : "Fail");
            Console.WriteLine();

            string result = SyntaxAnalyzer.Parse(resLexemes.Item2);
            Console.WriteLine(result);
        }
    }
}