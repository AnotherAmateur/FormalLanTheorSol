﻿using Lab5_Lexical_Analyzer;
using System.Linq;

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

            Console.WriteLine();

            if (resLexemes.Item1 is false)
            {
                Console.WriteLine("FAIL INFO:");
                Console.WriteLine($" Value: {LexAnalyzer.ErrorInfo.Value,-8} | " +
                                   $"Position: [{LexAnalyzer.ErrorInfo.LinePos}/" +
                                   $"{LexAnalyzer.ErrorInfo.LexemePos}/" +
                                   $"{LexAnalyzer.ErrorInfo.CharPos}]");
                return;
            }

            Console.WriteLine("LexAnalyzer: SUCCESS");
            Console.WriteLine();

            string result = SyntaxSemanticAnalyzer.Parse(resLexemes.Item2);
            Console.WriteLine(result);

            Console.WriteLine();
            foreach (var (x, i) in SyntaxSemanticAnalyzer.Poliz.Select((x, i) => (x, i)))
            {
                Console.Write($"{i}:");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(x.Value);
                Console.ResetColor(); 
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
}