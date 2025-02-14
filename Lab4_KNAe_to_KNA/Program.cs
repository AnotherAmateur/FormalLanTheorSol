// Преобразование НКА с эпсилон переходами к простому НКА

namespace Lab4_KNAe_to_KNA
{
    public class Program
    {
        public static void Main()
        {
            string configFilePath = "D:\\Uni\\4course_1sem\\FormalLanTheorSol\\Projects\\FormalLanTheorSol\\Lab4_KNAe_to_KNA\\automaton.me";

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
                            automaton.PrintTransMatrix();
                            break;
                        case 2:
                            {
                                Console.Write("Word: _\b");
                                string? word = Console.ReadLine();

                                automaton.Exec(word ?? "");

                                Console.WriteLine("------------------------------");
                                foreach (var record in automaton.Logs)
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