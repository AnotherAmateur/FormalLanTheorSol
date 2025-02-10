using Lab5_Lexical_Analyzer;

namespace Lab7_Syntax_Analyzer
{
    public class Program
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

            string result = SyntaxSemanticAnalyzer.Parse(resLexemes.Item2);
            Console.WriteLine(result);

            Console.Write(string.Join(" ", SyntaxSemanticAnalyzer.PolizStack.Reverse()));

            Console.WriteLine();
        }
    }
}