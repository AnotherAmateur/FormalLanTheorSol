using System.Text;

namespace FormalLanTheor
{
    public class Automat
    {
        const string PassSymb = "-";

        string? initState;
        List<string> finalStates;
        List<char> alphabet;
        Dictionary<string, Dictionary<char, List<string>>> transMatrix;


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
                Environment.Exit(-1);
            }
        }

        public List<string> Exec(string word)
        {
            List<string> logs = new();
            if (ExecParallel(word, initState, logs, 1))
            {
                logs.Sort();
                logs.Add("Конечное состояние достигнуто");
            }
            else
            {
                logs.Sort();
                logs.Add("Конечное состояние НЕ достигнуто");
            }


            return logs;
        }


        public bool ExecParallel(string word, string state, List<string> logs, int step)
        {
            if (word.Length == 0)
            {
                return finalStates.Contains(state);
            }

            char symbol = word[0];
            string remainingInput = word.Substring(1);

            if (!transMatrix.ContainsKey(state))
            {
                return false;
            }

            if (!transMatrix[state].ContainsKey(symbol))
            {
                return false;
            }

            var nextStates = transMatrix[state][symbol];

            bool result = false;

            Parallel.ForEach(nextStates, nextState =>
            {
                if (ExecParallel(remainingInput, nextState, logs, step + 1))
                {
                    result = true;
                }

                lock (logs)
                {
                    logs.Add($"{(result ? "*" : "")}Step: {step}, State: {state}, Symbol: {symbol}, Next State: {nextState}");
                }
            });

            return result;
        }

        public void PrintConfigFile()
        {
            Console.WriteLine();

            Console.WriteLine($"Alphabet: {string.Join(", ", alphabet)}");
            Console.WriteLine($"States: {string.Join(", ", transMatrix.Keys)}");
            Console.WriteLine($"Initial state: {initState}");
            Console.WriteLine($"Final state(s): {string.Join(", ", finalStates)}");
            Console.WriteLine("Transition matrix:");
            Console.WriteLine($"{new string(' ', 4)}\t{string.Join('\t', alphabet)}");

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

                Console.Write($"{pred.PadLeft(3)}{line.Key} |\t");

                foreach (var item in line.Value.Values.Select(x => string.Join(',', x)))
                {
                    Console.Write($"{{{item}}}\t");
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
