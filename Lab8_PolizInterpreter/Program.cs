using Lab7_Syntax_Analyzer_Poliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab8_PolizInterpreter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Lab7_Syntax_Analyzer_Poliz.Program.Main(null);

            Console.WriteLine("\nИнтерпретация\n");
            try
            {
                PolizInterpreter.Execute(Lab7_Syntax_Analyzer_Poliz.SyntaxAnalyzerPoliz.Poliz);
            }
            catch (Exception ex)
            {
                Print();
                Console.WriteLine($"Вызвано исключение: {ex.Message}");
                return;
            }

            Print();
            Console.WriteLine("Завершено успешно");
        }

        public static void Print()
        {
            Console.WriteLine(new string('-', 90));
            Console.WriteLine($"| Шаг{null,-2}| Инструкция{null,-6} | Стек{null,-10} | Переменные{null,-30}");
            Console.WriteLine(new string('-', 90));

            foreach (var log in PolizInterpreter.ExecutionLogs)
            {
                string stackStr = string.Join(", ", log.StackSnapshot);
                string varsStr = string.Join(", ", log.VariablesSnapshot.Select(v => $"{v.Key}: {v.Value}"));
                Console.WriteLine($"| {log.Step,-4} | {log.Instruction,-16} | {stackStr,-14} | {varsStr,-30}");
            }
            Console.WriteLine(new string('-', 90));
        }
    }
}
