using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;

namespace Lab4_KNA_eps
{
    public class Automat
    {
        const string PassSymb = "-";
        const string EpsSymb = "Eps";

        string? initState;
        List<string> finalStates;
        List<char> alphabet;
        Dictionary<string, Dictionary<string, List<string>>> transMatrix;


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

        public List<string> ExecuteNFAWithEpsilonTransitions(string word)
        {
            var logs = new List<string>();
            var statesCurrentStack = new Stack<string>();
            statesCurrentStack.Push(initState);

            string AddEpsilonTransitions(string state)
            {
                if (finalStates.Contains(state))
                {
                    return state;
                }

                if (!transMatrix[state][EpsSymb].First().Equals(PassSymb))
                {
                    foreach (var nextState in transMatrix[state][EpsSymb])
                    {
                        logs.Add($"Эпсилон-переход из состояния {{{state}}} в состояние {{{nextState}}}");

                        if (!statesCurrentStack.Contains(nextState))
                        {
                            statesCurrentStack.Push(nextState);
                            string finalState = AddEpsilonTransitions(nextState);

                            if (finalState != null)
                            {
                                return finalState;
                            }
                        }
                    }
                }

                return null;
            }

            var statesNextSet = new HashSet<string>();
            var logsTemp = new Queue<string>();

            foreach (char letter in word)
            {
                while (statesCurrentStack.Any())
                {
                    string stateCurrent = statesCurrentStack.Pop();
                    string finalState = AddEpsilonTransitions(stateCurrent);

                    if (finalState != null)
                    {
                        logs.Add($"Достигнуто конечное состояние {{{finalState}}}");
                        return logs;
                    };

                    if (alphabet.Contains(letter))
                    {
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
                }

                if (!alphabet.Contains(letter))
                {
                    logs.Add($"Неверный символ '{letter}'");
                    break;
                }

                logs.AddRange(logsTemp);
                statesCurrentStack = new Stack<string>(statesNextSet);
            }

            while (statesCurrentStack.Any())
            {
                string stateCurrent = statesCurrentStack.Pop();
                string finalState = AddEpsilonTransitions(stateCurrent);

                if (finalState != null)
                {
                    logs.Add($"Достигнуто конечное состояние {{{finalState}}}");
                    return logs;
                };
            }

            logs.Add("Конечное состояние НЕ достигнуто");
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
