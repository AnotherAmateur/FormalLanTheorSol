﻿// Недетерминированный конечный автомат

namespace FormalLanTheor
{
    public class Program
    {
        public static void Main()
        {
            string configFilePath = "D:\\Uni\\4course_1sem\\FormalLanTheorSol\\Projects\\FormalLanTheorSol\\Lab2_KNA\\automaton.me";

            var automaton = new Automat(configFilePath);

            while (true)
            {
                WelcomeMenu();

                int choice;
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            automaton.PrintConfigFile();
                            break;
                        case 2:
                            {
                                Console.Write("Word: _\b");
                                string? word = Console.ReadLine();

                                List<string> resultLogs = automaton.Exec(word ?? "");

                                Console.WriteLine("------------------------------");
                                foreach (var record in resultLogs)
                                {
                                    Console.WriteLine(record);
                                }
                                Console.WriteLine("------------------------------");
                            }
                            break;
                    }
                    continue;
                }

                Console.WriteLine("Bad input");
            }
        }

        private static void WelcomeMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Press 1 to see the automaton info");
            Console.WriteLine("Press 2 to enter a word");
        }
    }
}