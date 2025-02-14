using Lab7_Syntax_Analyzer_Poliz;
using Lab7_Syntax_Analyzer_Poliz.Enums;

namespace Lab8_PolizInterpreter
{
    public static class PolizInterpreter
    {
        private static Lab7_Syntax_Analyzer_Poliz.PostfixEntry PostfixEntry;
        private static Dictionary<string, int> _variables;
        private static Stack<object> _stack;
        private static List<ExecutionLog> _executionLogs;
        public static List<ExecutionLog> ExecutionLogs { get => _executionLogs; }
        private static string _loopVar;

        private static int GetValueOfOperand(object operandObj)
        {
            string operandStr = operandObj.ToString();

            // переменные сохраняются только при их использовании в правой части выражений
            if (_variables.ContainsKey(operandStr))
            {
                return _variables[operandStr];
            }
            else if (operandObj is string)
            {
                Console.Write($"Значение для {operandStr}: _\b");
                int value = int.Parse(Console.ReadLine());
                _variables.Add(operandStr, value);
                return value;
            }

            return Convert.ToInt32(operandStr);
        }

        public static void Execute(List<PostfixEntry> poliz)
        {
            _variables = new();
            _stack = new();
            _executionLogs = new();

            int i = 0;
            while (i < poliz.Count)
            {
                var entry = poliz[i];
                string instruction = $"[{i}] {entry.Value}";

                switch (entry.Type)
                {
                    case EntryType.Var:
                        string varName = entry.Value.ToString();
                        _stack.Push(varName);
                        break;
                    case EntryType.Const:
                        _stack.Push(Convert.ToInt32(entry.Value));
                        break;
                    case EntryType.Cmd:
                        ExecuteCommand((Cmd)entry.Value, ref i);
                        break;
                    case EntryType.CmdPtr:
                        _stack.Push(Convert.ToInt32(entry.Value));
                        break;
                }

                _executionLogs.Add(new(_executionLogs.Count, instruction, _stack.Reverse().ToList(), new(_variables)));
                ++i;
            }
        }

        private static void ExecuteCommand(Cmd cmd, ref int i)
        {
            object operandA;
            object operandB;
            switch (cmd)
            {
                case Cmd.SET:
                    operandB = _stack.Pop();
                    operandA = _stack.Pop();

                    string varName = operandA.ToString();
                    if (_variables.ContainsKey(varName) is false)
                        _variables.Add(varName, GetValueOfOperand(operandB));

                    if (_loopVar is null) { _loopVar = varName; }
                    break;
                case Cmd.ADD:
                    operandB = _stack.Pop();
                    operandA = _stack.Pop();
                    _stack.Push(GetValueOfOperand(operandA) + GetValueOfOperand(operandB));
                    break;
                case Cmd.SUB:
                    operandB = _stack.Pop();
                    operandA = _stack.Pop();
                    _stack.Push(GetValueOfOperand(operandA) - GetValueOfOperand(operandB));
                    break;
                case Cmd.CMPLE:
                    operandB = _stack.Pop();
                    operandA = _stack.Pop();
                    _stack.Push(GetValueOfOperand(operandA) <= GetValueOfOperand(operandB) ? 1 : 0);
                    break;
                case Cmd.JZ:
                    operandB = _stack.Pop();
                    operandA = _stack.Pop();
                    i = GetValueOfOperand(operandA) == 0 ? GetValueOfOperand(operandB) : i;
                    break;
                case Cmd.JMP:
                    operandA = _stack.Pop();
                    ++_variables[_loopVar];
                    i = GetValueOfOperand(operandA) - 1;  // -1 т.к. после будет выполнен инкремент
                    break;
            }
        }
    }

    public struct ExecutionLog
    {
        public int Step;
        public string Instruction;
        public List<object> StackSnapshot;
        public Dictionary<string, int> VariablesSnapshot;

        public ExecutionLog(int step, string instruction, List<object> stackSnapshot, Dictionary<string, int> variablesSnapshot)
        {
            Step = step;
            Instruction = instruction ?? throw new ArgumentNullException(nameof(instruction));
            StackSnapshot = stackSnapshot ?? throw new ArgumentNullException(nameof(stackSnapshot));
            VariablesSnapshot = variablesSnapshot ?? throw new ArgumentNullException(nameof(variablesSnapshot));
        }
    }
}
