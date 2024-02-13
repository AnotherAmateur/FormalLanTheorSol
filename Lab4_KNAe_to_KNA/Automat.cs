using Microsoft.VisualBasic;
using System.Text;

namespace Lab4_KNAe_to_KNA
{
    public class Automat
    {
        private const string PassSymb = "-";
        private const string EpsSymb = "Eps";

        private string? initState;
        private List<string> finalStates;
        private List<char> alphabet;
        private Dictionary<string, Dictionary<string, List<string>>> transMatrix;
        private bool forbiddenTrans;
        private bool result;
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

            ConvertToKNA();
        }


        public void Exec(string word)
        {
            Logs = new();
            ExecParallel(word, initState, 1);
            Logs.Sort();
            string msg = result && !forbiddenTrans ? "СЛОВО ПРИНЯТО" : "СЛОВО НЕ ПРИНЯТО";
            Logs.Add(msg);

            result = false;
            forbiddenTrans = false;
        }

        public void ExecParallel(string word, string state, int step)
        {
            if (finalStates.Contains(state))
            {
                result = true;
            }

            if (word.Length == 0)
            {
                return;
            }

            var symbol = word[0].ToString();
            string remainingInput = word.Substring(1);

            if (!transMatrix.ContainsKey(state))
            {
                forbiddenTrans = true;
                return;
            }

            if (!transMatrix[state].ContainsKey(symbol))
            {
                forbiddenTrans = true;
                return;
            }

            var nextStates = transMatrix[state][symbol].Except(new List<string>() { PassSymb }).ToList();

            if (nextStates.Count == 0)
            {
                forbiddenTrans = true;
            }

            Parallel.ForEach(nextStates, nextState =>
            {
                ExecParallel(remainingInput, nextState, step + 1);

                lock (Logs)
                {
                    Logs.Add($"{(finalStates.Contains(nextState) ? "*" : "")}Шаг: {step}. Из состояния: {state} в состояние: {nextState} по символу: {symbol}");
                }
            });
        }

        private void ConvertToKNA()
        {
            foreach (var state in transMatrix.Keys)
            {
                var epsClosure = new HashSet<String>();
                FindEpsClosureRec(state, epsClosure);

                foreach (var stateByEps in epsClosure)
                {
                    if (finalStates.Contains(stateByEps) && finalStates.Contains(state) is false)
                    {
                        finalStates.Add(state);
                    }

                    AddNewTransitions(stateByEps, state);
                }
            }

            foreach (var state in transMatrix.Keys)
            {
                transMatrix[state].Remove(EpsSymb);
            }
        }

        private void AddNewTransitions(string fromState, string toState)
        {
            foreach (var letter in alphabet)
            {
                var letterStr = letter.ToString();
                var temp = transMatrix[fromState][letterStr].Except(transMatrix[toState][letterStr]).ToList();
                transMatrix[toState][letterStr].AddRange(temp);

                if (temp.Count() > 0)
                {
                    transMatrix[toState][letterStr].Remove(PassSymb);
                }
            }
        }

        private void FindEpsClosureRec(string state, HashSet<String> resultSet)
        {
            if (state == PassSymb)
            {
                return;
            }

            resultSet.Add(state);

            List<string> nextStates = transMatrix[state][EpsSymb];
            foreach (var nextState in nextStates)
            {
                FindEpsClosureRec(nextState, resultSet);
            }
        }

        private bool ValidateLetter(char letter)
        {
            if (!alphabet.Contains(letter))
            {
                Logs.Add($"Неверный символ '{letter}'");
                Logs.Add("СЛОВО НЕ ПРИНЯТО");
                return false;
            }

            return true;
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
