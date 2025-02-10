using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormalLanTheor
{
    public class Automat
    {
        const string PassSymb = "-";

        string? initState;
        List<string> finalStates;
        List<char> alphabet;
        Dictionary<string, Dictionary<char, string>> transMatrix;


        public Automat(string path)
        {
            initState = null;
            finalStates = new();
            alphabet = new();
            transMatrix = new();

            try
            {
                ReadFileConfig(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File reading failed. Message: {ex.Message}");
            }
        }


        public List<string> Exec(string word)
        {
            List<string> logs = new();

            string curState = initState;
            int i = word.Length;

            foreach (char letter in word)
            {
                if (alphabet.Contains(letter) is false)
                {
                    logs.Add($"The given sumbol \"{letter}\" is out of alphabet.");
                    logs.Add("The given word is rejected.");
                    return logs;
                }

                string nextState = transMatrix[curState][letter];

                if (nextState.Equals(PassSymb))
                {
                    logs.Add("Can`t proceed the transition.");
                    logs.Add("The given word is rejected.");
                    return logs;
                }

                logs.Add($"{letter}: {curState} -> {nextState}");
                curState = nextState;
            }

            if (finalStates.Contains(curState))
            {
                logs.Add("The given word is accepted");
            }
            else
            {
                logs.Add("The given word is rejected");
            }

            return logs;
        }

        public void PrintConfigFile()
        {
            Console.WriteLine();

            Console.WriteLine($"Alphabet: {string.Join(", ", alphabet)}");
            Console.WriteLine($"States: {string.Join(", ", transMatrix.Keys)}");
            Console.WriteLine($"Initial state: {initState}");
            Console.WriteLine($"Final state(s): {string.Join(", ", finalStates)}");

            Console.WriteLine("Transition matrix:");
            Console.WriteLine($"{new string(' ', 4)}\t{string.Join("  ", alphabet)}");
            foreach (var line in transMatrix)
            {
                string pred = new string(' ', 3);

                if (initState.Contains(line.Key))
                {
                    pred = "->";
                }
                if (finalStates.Contains(line.Key))
                {
                    pred += (pred.Last() == ' ') ? "\b*" : "*";
                }

                Console.WriteLine($"{pred}{line.Key} |\t{string.Join("  ", line.Value.Values)}");
            }

            Console.WriteLine();
        }


        private void ReadFileConfig(string path)
        {
            using (var configFyle = new StreamReader(path, Encoding.UTF8))
            {
                string? line = configFyle.ReadLine();
                if (line is null)
                {
                    // todo
                    throw new Exception();
                }

                // чтение алфавита
                foreach (string letter in line.Split('/', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (letter.Length != 1)
                    {
                        // todo
                        throw new Exception();
                    }

                    alphabet.Add(char.Parse(letter));
                }

                // чтение числа состояний
                int stateCount;
                if (int.TryParse(configFyle.ReadLine(), out stateCount) is false)
                {
                    // todo
                    throw new Exception();
                }

                // чтение переходов состояний
                for (int i = 0; i < stateCount; ++i)
                {
                    line = configFyle.ReadLine();
                    if (line is null)
                    {
                        // todo
                        throw new Exception();
                    }

                    string[] transitions = line.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    transMatrix.Add(transitions[0], new());

                    if (transitions.Length - 1 != alphabet.Count)
                    {
                        // todo
                        throw new Exception();
                    }

                    for (int j = 1; j < transitions.Length; ++j)
                    {
                        transMatrix[transitions[0]].Add(alphabet[j - 1], transitions[j]);
                    }
                }

                if (transMatrix.Count == 0)
                {
                    // todo
                    throw new Exception();
                }

                // установка начального состояния
                initState = configFyle.ReadLine();
                if (initState is null || transMatrix.ContainsKey(initState) is false)
                {
                    // todo
                    throw new Exception();
                }

                // установка конечных состояний
                string? fStatesLine = configFyle.ReadLine();
                if (fStatesLine is null)
                {
                    // todo
                    throw new Exception();
                }

                finalStates = fStatesLine.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (finalStates.All(transMatrix.ContainsKey) is false)
                {
                    // todo
                    throw new Exception();
                }
            }
        }
    }
}
