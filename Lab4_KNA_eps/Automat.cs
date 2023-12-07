using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;

namespace Lab4_KNA_eps
{
    public class Automat
    {
        private const string PassSymb = "-";
        private const string EpsSymb = "Eps";

        private string? initState;
        private List<string> finalStates;
        private List<char> alphabet;
        private Dictionary<string, Dictionary<string, List<string>>> transMatrix;
        public List<string> Logs { get; private set; }

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

        public void ExecuteNFAWithEpsilonTransitions(string word)
        {
            Logs = new();
            var statesCurrentStack = new Stack<string>();
            statesCurrentStack.Push(initState);
            string finalState = null;

            void AddEpsilonTransitions(string state)
            {
                if (finalStates.Contains(state))
                {
                    finalState = state;
                }

                if (!transMatrix[state][EpsSymb].First().Equals(PassSymb))
                {
                    foreach (var nextState in transMatrix[state][EpsSymb])
                    {
                        Logs.Add($"Эпсилон-переход из состояния {{{state}}} в состояние {{{nextState}}}");

                        if (!statesCurrentStack.Contains(nextState))
                        {
                            statesCurrentStack.Push(nextState);
                            AddEpsilonTransitions(nextState);
                        }
                    }
                }
            }

            int position = -1;
            foreach (char letter in word)
            {
                ++position;
                var logsTemp = new Queue<string>();
                var statesNextSet = new HashSet<string>();

                ValidateLetter(letter);

                foreach (var state in new Stack<string>(statesCurrentStack))
                {
                    AddEpsilonTransitions(state);
                }

                while (statesCurrentStack.Any())
                {
                    string stateCurrent = statesCurrentStack.Pop();

                    if (transMatrix[stateCurrent][letter.ToString()].First().Equals(PassSymb))
                    {
                        continue;
                    }

                    foreach (string stateNext in transMatrix[stateCurrent][letter.ToString()])
                    {
                        statesNextSet.Add(stateNext);
                        logsTemp.Enqueue($"Из состояния {{{stateCurrent}}} в состояние {{{stateNext}}} по слову '{letter}'");
                    }
                }

                if (!logsTemp.Any())
                {
                    Logs.Add($"Невозможно совершить переход по текущему символу: {letter}, позиция в слове: {position}");
                    return;
                }

                Logs.AddRange(logsTemp);
                statesCurrentStack = new Stack<string>(statesNextSet);
            }

            foreach (var state in new Stack<string>(statesCurrentStack))
            {
                AddEpsilonTransitions(state);
            }

            string result = finalState is null ? "СЛОВО НЕ ПРИНЯТО" : "СЛОВО ПРИНЯТО";
            Logs.Add(result);

            return;
        }

        private void ValidateLetter(char letter)
        {
            if (!alphabet.Contains(letter))
            {
                Logs.Add($"Неверный символ '{letter}'");
                Logs.Add("СЛОВО НЕ ПРИНЯТО");
                throw new Exception();
            }
        }

        public void PrintConfigFile()
        {
            Console.WriteLine();

            Console.WriteLine($"Alphabet: {string.Join(", ", alphabet)}");
            Console.WriteLine($"States: {string.Join(", ", transMatrix.Keys)}");
            Console.WriteLine($"Initial state: {initState}");
            Console.WriteLine($"Final state(s): {string.Join(", ", finalStates)}");
            Console.WriteLine("Transition matrix:");
            Console.WriteLine($"{new string(' ', 4)}\t{string.Join('\t', alphabet)}\t{EpsSymb}");

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
                    throw new NotImplementedException();
                }

                // чтение алфавита
                foreach (string letter in line.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (letter.Length != 1)
                    {
                        throw new NotImplementedException();
                    }

                    alphabet.Add(char.Parse(letter));
                }

                // чтение числа состояний
                int stateCount;
                if (int.TryParse(configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                    out stateCount) is false)
                {
                    throw new NotImplementedException();
                }

                // чтение переходов состояний
                for (int i = 0; i < stateCount; ++i)
                {
                    line = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (line is null)
                    {
                        throw new NotImplementedException();
                    }

                    string[] transitions = line.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    transMatrix.Add(transitions[0], new());

                    if (transitions.Length - 2 != alphabet.Count)
                    {
                        throw new NotImplementedException();
                    }

                    for (int j = 1; j < transitions.Length - 1; ++j)
                    {
                        transMatrix[transitions[0]].Add(alphabet[j - 1].ToString(),
                            transitions[j].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                    }

                    transMatrix[transitions[0]].Add(EpsSymb, transitions.Last().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                }

                if (transMatrix.Count == 0)
                {
                    throw new NotImplementedException();
                }

                // установка начального состояния
                initState = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (initState is null || transMatrix.ContainsKey(initState) is false)
                {
                    throw new NotImplementedException();
                }

                // установка конечных состояний
                string? fStatesLine = configFyle.ReadLine()?.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (fStatesLine is null)
                {
                    throw new NotImplementedException();
                }

                finalStates = fStatesLine.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (finalStates.All(transMatrix.ContainsKey) is false)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
