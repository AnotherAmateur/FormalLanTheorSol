using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormalLanTheor
{
    public class Automat
    {
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

                foreach (string letter in line.Split('/', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (letter.Length != 1)
                    {
                        // todo
                        throw new Exception();
                    }

                    alphabet.Add(char.Parse(letter));
                }

                int stateCount;
                if (int.TryParse(configFyle.ReadLine(), out stateCount) is false)
                {
                    // todo
                    throw new Exception();
                }

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

                    for (int j = 1; j < transitions.Length; ++j)
                    {
                        if (letter.Length != 1)
                        {

                        }

                        alphabet.Add(char.Parse(letter));
                    }
                }



            }
        }

    }
}
