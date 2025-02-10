using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FormalLanTheor
{
    public class Automat
    {
        const string PassSymb = "-";
        const string Accept = "ACCEPTED";
        const string Reject = "REJECTED";

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

        public List<string> ExecNew(string word)
        {
            List<string> logs = new();
            List<string> stateCur = new();
            stateCur.Add(initState);

            bool hasError = false;
            foreach (char symbol in word)
            {
                if (alphabet.Contains(symbol) is false)
                {
                    logs.Add($"The given symbol \"{symbol}\" is out of alphabet.");
                    hasError = true;
                    break;
                }

                List<string> nextStatesOnSymb = new();
                foreach (string state in stateCur)
                {
                    nextStatesOnSymb.AddRange(transMatrix[state][symbol].Except(new string[] { PassSymb }));
                }

                if (nextStatesOnSymb.Count == 0)
                {
                    logs.Add($"Can`t proceed transition.");
                    hasError = true;
                    break;
                }

                string stateCurStr = GetStrForState(stateCur);
                string stateNextStr = GetStrForState(nextStatesOnSymb);

                // добавление в матрицу нового состояния и перехода
                if (transMatrix[stateCurStr].ContainsKey(symbol) is false)
                {
                    transMatrix[stateCurStr].Add(symbol, nextStatesOnSymb);
                }
                if (transMatrix.ContainsKey(stateNextStr) is false)
                {
                    transMatrix.Add(stateNextStr, new());
                }

                logs.Add($"From state {{{stateCurStr}}} to {{{stateNextStr}}} on symbol \'{symbol}\'");
                stateCur = nextStatesOnSymb;
            }

            string result = (hasError is false && stateCur.Intersect(finalStates).Any()) ? Accept : Reject;
            logs.Add(result);
            return logs;
        }

        private string GetStrForState(IEnumerable<string> list)
        {
            return $"{string.Join(',', list)}";
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

                Console.Write($"{(pred + $"{{{line.Key}}}").PadLeft(Margin)} | ");

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
