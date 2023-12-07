using Lab5_Lexical_Analyzer;

namespace Lab6_Syntax_Analyzer
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


            Console.WriteLine("Index | Category    | Type        | Value    | Line/Lexeme/Char");
            Console.WriteLine(new string('-', 67));

            for (int i = 0; i < resLexemes.Item2.Count; i++)
            {
                Console.WriteLine($"{i + ".",-5} | " +
                                  $"{resLexemes.Item2[i].LexCat,-11} | " +
                                  $"{resLexemes.Item2[i].LexType,-11} | " +
                                  $"{resLexemes.Item2[i].Value,-8} | " +
                                  $"[{resLexemes.Item2[i].LinePos}/{resLexemes.Item2[i].LexemePos}/{resLexemes.Item2[i].CharPos}]");
            }

            Console.WriteLine(resLexemes.Item1 ? "Success" : "Fail");
            Console.WriteLine();

            if (resLexemes.Item1)
            {
                string result = SyntaxAnalyzer.Parse(resLexemes.Item2);
                Console.WriteLine(result);
            }
        }
    }
}