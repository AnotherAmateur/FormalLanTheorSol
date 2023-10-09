using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FormalLanTheor
{
    public class Automat
    {
        const string PassSymb = "-";
        const string Accept = "The final state was reached";
        const string Reject = "The final state wasn`t reached";

        string initState;
        List<string> finalStates;
        List<char> alphabet;
        Dictionary<string, Dictionary<char, List<string>>> transMatrix;

        public Automat(string path)
        {
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
                Environment.Exit(-1);
            }
        }

        public List<string> Exec(string word)
        {
            List<string> logs = new();
            if (word.Length == 0 && finalStates.Contains(initState))
            {
                logs.Add(Accept);
            }
            else
            {
                logs.Add(Exec(word, initState, logs) ? Accept : Reject);
            }

            return logs;
        }

        public bool Exec(string word, string curState, List<string> logs)
        {
            if (word.Length != 0)
            {
                char curLetter = word[0];
                string remainingInput = word.Substring(1);
                if (curState == PassSymb || !alphabet.Contains(curLetter))
                    return false;

                string[] stateSplitted = curState.Split(',', StringSplitOptions.RemoveEmptyEntries);

                // парсинг всех переходов для нового состояния
                if (transMatrix.ContainsKey(curState) is false)
                {
                    foreach (var state in stateSplitted)
                    {
                        if (finalStates.Contains(state))
                        {
                            transMatrix.Add($"*{curState}", new());
                            return true;
                        }
                    }

                    transMatrix.Add(curState, new());

                    foreach (var letter in alphabet)
                    {
                        List<string> nextStatesOnLetter = new();

                        foreach (var state in stateSplitted)
                        {
                            nextStatesOnLetter.AddRange(transMatrix[state][letter].Except(new string[] { "-" }));
                        }

                        if (nextStatesOnLetter.Count == 0)
                            nextStatesOnLetter.Add(PassSymb);

                        transMatrix[curState][letter] = nextStatesOnLetter;
                    }
                }

                string nextStateStr = string.Join(',', transMatrix[curState][curLetter].Except(new string[] { "-" }));

                if (nextStateStr != string.Empty)
                {
                    logs.Add($"From state {{{curState}}} to {{{nextStateStr}}} on letter {curLetter}");
                    return Exec(remainingInput, nextStateStr, logs);
                }
                else
                {
                    logs.Add($"From state {{{curState}}} to {{-}} on letter {curLetter}");
                }
            }

            return false;
        }

        public void PrintConfigFile()
        {
            const int Margin = 10;
            Console.WriteLine();

            Console.WriteLine($"Alphabet: {string.Join(", ", alphabet)}");
            Console.WriteLine($"States: {string.Join(", ", transMatrix.Keys)}");
            Console.WriteLine($"Initial state: {initState}");
            Console.WriteLine($"Final state(s): {string.Join(", ", finalStates)}");
            Console.WriteLine("Transition matrix:");
            Console.WriteLine($"{new string(' ', Margin + 4)}{string.Join(new string(' ', Margin - 1), alphabet)}");

            foreach (var line in transMatrix)
            {
                string pred = "";

                if (initState.Contains(line.Key))
                {
                    pred = "->";
                }
                if (finalStates.Contains(line.Key))
                {
                    pred += "*";
                }

                Console.Write($"{(pred + line.Key).PadLeft(Margin)} | ");

                foreach (var item in line.Value.Values.Select(x => string.Join(',', x)))
                {
                    string t = $"{{{item}}}";
                    Console.Write(t.PadRight(Margin));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private void ReadFileConfig(string path)
        {
            using (var configFyle = new StreamReader(path, Encoding.UTF8))
            {
                string? line = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (line is null)
                {
                    // todo
                    throw new Exception();
                }

                // чтение алфавита
                foreach (string letter in line.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries))
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
                if (int.TryParse(configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                    out stateCount) is false)
                {
                    // todo
                    throw new Exception();
                }

                // чтение переходов состояний
                for (int i = 0; i < stateCount; ++i)
                {
                    line = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (line is null)
                    {
                        // todo
                        throw new Exception();
                    }

                    string[] transitions = line.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    transMatrix.Add(transitions[0], new());

                    if (transitions.Length - 1 != alphabet.Count)
                    {
                        // todo
                        throw new Exception();
                    }

                    for (int j = 1; j < transitions.Length; ++j)
                    {
                        transMatrix[transitions[0]].Add(alphabet[j - 1],
                            transitions[j].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                    }
                }

                if (transMatrix.Count == 0)
                {
                    // todo
                    throw new Exception();
                }

                // установка начального состояния
                initState = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (initState is null || transMatrix.ContainsKey(initState) is false)
                {
                    // todo
                    throw new Exception();
                }

                // установка конечных состояний
                string? fStatesLine = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (fStatesLine is null)
                {
                    // todo
                    throw new Exception();
                }

                finalStates = fStatesLine.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (finalStates.All(transMatrix.ContainsKey) is false)
                {
                    // todo
                    throw new Exception();
                }
            }
        }
    }
}
