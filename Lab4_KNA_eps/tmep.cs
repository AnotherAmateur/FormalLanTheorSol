using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4_KNA_eps
{

    public class tmep
    {
        const string PassSymb = "-";
        const string EpsSymb = "Eps";

        private string? currentState;
        private List<string> finalStates;
        private List<char> alphabet;
        private Dictionary<string, Dictionary<string, List<string>>> transMatrix;

        public tmep(string? initState, List<string> finalStates, List<char> alphabet, Dictionary<string, Dictionary<string, List<string>>> transMatrix)
        {
            this.currentState = initState;
            this.finalStates = finalStates;
            this.alphabet = alphabet;
            this.transMatrix = transMatrix;
        }

        public bool ProcessInput(string input)
        {
            // Инициализация стека для отслеживания возможных состояний
            Stack<string> stateStack = new Stack<string>();
            stateStack.Push(currentState);

            // Функция для добавления эпсилон-переходов в стек
            void AddEpsilonTransitions(string state)
            {
                if (!transMatrix[state][EpsSymb].First().Equals(PassSymb))
                {
                    foreach (var nextState in transMatrix[state][EpsSymb])
                    {
                        Console.WriteLine($"Epsilon Transition: {state} -> {nextState}");
                        if (!stateStack.Contains(nextState))
                        {
                            stateStack.Push(nextState);
                        }
                       
                        AddEpsilonTransitions(nextState);
                    }
                }
            }

            // Обработка входной строки
            foreach (char symbol in input)
            {
                Stack<string> nextStates = new Stack<string>();

                // Добавление новых состояний с учетом эпсилон-переходов
                while (stateStack.Count > 0)
                {
                    string currentState = stateStack.Pop();
                    if (transMatrix.ContainsKey(currentState) && transMatrix[currentState].ContainsKey(symbol.ToString()))
                    {
                        foreach (var nextState in transMatrix[currentState][symbol.ToString()])
                        {
                            Console.WriteLine($"Transition: {currentState} -> {nextState} (Symbol: {symbol})");
                            nextStates.Push(nextState);
                            AddEpsilonTransitions(nextState);
                        }
                    }
                }

                stateStack = nextStates;
            }

            // Проверка, достигнуто ли конечное состояние
            while (stateStack.Count > 0)
            {
                string currentState = stateStack.Pop();
                if (finalStates.Contains(currentState))
                {
                    Console.WriteLine($"String accepted at final state: {currentState}");
                    return true;
                }
            }

            Console.WriteLine("String rejected.");
            return false;
        }
    }

}