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

        public Dictionary<string, string> GetEpsilonClosure(string state)
        {
            var epsilonClosure = new Dictionary<string, string>();

            void EpsilonClosureDFS(string curState)
            {
                if (epsilonClosure.ContainsKey(curState))
                {
                    return;
                }

                epsilonClosure.Add(curState, curState);

                foreach (string nextState in transMatrix[curState][EpsSymb])
                {               
                    epsilonClosure.Add(nextState, curState);
                    EpsilonClosureDFS(nextState);
                }
            }

            EpsilonClosureDFS(state);

            return epsilonClosure;
        }

        public List<string> ExecuteNFAWithEpsilonTransitions(string word)
        {
            var logs = new List<string>();
            var currentStates = new HashSet<string>();

            bool NFAWithEpsilonTransitionsDFS(string curState, string remainingInput, int step)
            {
                Dictionary<string, string> epsTransitions = GetEpsilonClosure(curState);
                foreach (var transition in epsTransitions)
                {
                    logs.Add($"Из состояния {{{transition.Value}}} в состояние {{{transition.Key}}} по слову '{EpsSymb}'");

                    if (finalStates.Contains(transition.Key))
                    {
                        return true;
                    }
                }

                currentStates.UnionWith(epsTransitions.Keys);

                if (remainingInput.Length != 0)
                {
                    string symbol = remainingInput[0].ToString();
                    string newRemainingInput = remainingInput.Substring(1);

                    HashSet<string> nextStates = new HashSet<string>();

                    foreach (string state in currentStates)
                    {
                        if (transMatrix.ContainsKey(state) && transMatrix[state].ContainsKey(symbol))
                        {
                            nextStates.UnionWith(transMatrix[state][symbol]);
                        }
                    }

                    foreach (string nextState in nextStates)
                    {
                        logs.Add($"Из состояния {{{curState}}} в состояние {{{nextState}}} по слову '{symbol}'");
                        if (finalStates.Contains(nextState) || NFAWithEpsilonTransitionsDFS(nextState, newRemainingInput, step + 1))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            const string Accept = "Конечное состояние достигнуто";
            const string Reject = "Конечное состояние НЕ достигнуто";

            if (word.Length == 0 && finalStates.Contains(initState))
            {
                logs.Add(Accept);
            }
            else
            {
                logs.Add(NFAWithEpsilonTransitionsDFS(initState, word, 1) ? Accept : Reject);
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
