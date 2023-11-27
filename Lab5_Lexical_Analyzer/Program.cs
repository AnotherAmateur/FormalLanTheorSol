namespace Lab5_Lexical_Analyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            string code = @"for i = 1 + 2 to 5 
                                i = i == 1; 
                                i = i <> 1; 
                            next;";

            (bool, List<Lexeme>) resLexemes = LexAnalyzer.Analyze(code);

            for (int i = 0; i < resLexemes.Item2.Count; i++)
            {
                Console.WriteLine($"{i}. Category: {resLexemes.Item2[i].LexCat}, Type: {resLexemes.Item2[i].LexType}, " +
                    $"Value: {resLexemes.Item2[i].Value}");
            }
            Console.WriteLine(resLexemes.Item1 ? "Success" : "Fail");
            Console.WriteLine();
        }
    }
}