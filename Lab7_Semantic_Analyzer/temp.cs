using Lab5_Lexical_Analyzer.Enums;
using Lab5_Lexical_Analyzer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageTranslations
{
    class SyntaxAnalyzer
    {
        private List<Lexeme> _lexemes;
        private List<Operation> _operations;

        public List<Operation> Operations
        {
            get
            {
                return new List<Operation>(_operations);
            }
        }


        public SyntaxAnalyzer(LexemeAnalyzer analyzer)
        {
            _lexemes = analyzer.Lexemes;
            _operations = new List<Operation>();
        }

        public bool WhileStatement()
        {
            if (_lexemes is null)
            {
                throw new Exception("Invalid lexeme list");
            }

            int pos = 0;
            var lex = _lexemes[pos];

            if (lex is null || lex.LexType != ELexType.Do)
            {
                ShowError(pos, lex, "Ожидается оператор do");
                return false;
            }

            GetNewPositionAndLexeme(ref pos, ref lex);
            if (lex is null || lex.LexType != ELexType.While)
            {
                ShowError(pos, lex, "Ожидается оператор while");
                return false;
            }

            GetNewPositionAndLexeme(ref pos, ref lex);
            if (!Condition(ref pos, ref lex))
            {
                return false;
            }

            int JZAdrPos = _operations.Count;

            _operations.Add(new Operation() { Value = "JZ", Type = EOperationType.JZ }); ;


            if (!Operators(ref pos, ref lex))
            {
                return false;
            }

            if (lex is null || lex.LexType != ELexType.Loop)
            {
                ShowError(pos, lex, "Ожидается оператор loop");
                return false;
            }

            _operations.Add(new Operation() { Value = "0", Type = EOperationType.VAL });
            _operations.Add(new Operation() { Value = "JMP", Type = EOperationType.JMP });
            _operations.Insert(JZAdrPos, new Operation() { Value = (_operations.Count + 1).ToString(), Type = EOperationType.VAL });

            if (!GetNewPositionAndLexeme(ref pos, ref lex))
            {
                return true;
            }
            else
            {
                _operations = new List<Operation>();
                ShowError(pos, lex, "Лишние символы");
                return false;
            }
        }

        //Логическое выражение
        private bool Condition(ref int pos, ref Lexeme lex)
        {
            bool isNot = false;

            if (!(lex is null) && lex.LexType == ELexType.Not)
            {
                isNot = true;
                GetNewPositionAndLexeme(ref pos, ref lex);
            }

            if (!LogExpr(ref pos, ref lex))
            {
                return false;
            }

            while (!(lex is null) && lex.LexType == ELexType.Or)
            {
                var prevLex = lex;
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!LogExpr(ref pos, ref lex))
                {
                    return false;
                }

                _operations.Add(new Operation() { Value = "OR", Type = EOperationType.OR });
            }

            if (isNot)
            {
                _operations.Add(new Operation() { Value = "NOT", Type = EOperationType.NOT });
            }

            return true;
        }

        private bool LogExpr(ref int pos, ref Lexeme lex)
        {

            if (!RelExpr(ref pos, ref lex))
            {
                return false;
            }

            while (!(lex is null) && lex.LexType == ELexType.And)
            {
                var prevLex = lex;
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!RelExpr(ref pos, ref lex))
                {
                    return false;
                }

                _operations.Add(new Operation() { Value = "AND", Type = EOperationType.AND });
            }

            return true;
        }

        private bool RelExpr(ref int pos, ref Lexeme lex)
        {
            if (!Operand(ref pos, ref lex))
            {
                return false;
            }

            if (!(lex is null) && lex.LexType == ELexType.Relation)
            {
                var prevLex = lex;
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!Operand(ref pos, ref lex))
                {
                    return false;
                }

                switch (prevLex.Value)
                {
                    case "<":
                        {
                            _operations.Add(new Operation() { Value = "CMPL", Type = EOperationType.CMPL });
                            break;
                        }
                    case ">":
                        {
                            _operations.Add(new Operation() { Value = "CMPG", Type = EOperationType.CMPG });
                            break;
                        }
                    case "=":
                        {
                            _operations.Add(new Operation() { Value = "CMPE", Type = EOperationType.CMPE });
                            break;
                        }
                    case "<>":
                        {
                            _operations.Add(new Operation() { Value = "CMPNE", Type = EOperationType.CMPNE });
                            break;
                        }
                    case ">=":
                        {
                            _operations.Add(new Operation() { Value = "CMPGE", Type = EOperationType.CMPGE });
                            break;
                        }
                    case "<=":
                        {
                            _operations.Add(new Operation() { Value = "CMPLE", Type = EOperationType.CMPLE });
                            break;
                        }
                }
            }

            return true;
        }

        private bool Operand(ref int pos, ref Lexeme lex)
        {
            if (lex is null || (lex.LexType != ELexType.Variable && lex.LexType != ELexType.Constant))
            {
                ShowError(pos, lex, "Ожидается переменная или константа");
                return false;
            }

            if (lex.LexType == ELexType.Variable)
            {
                _operations.Add(new Operation() { Value = lex.Value, Type = EOperationType.VAR });
            }
            else
            {
                _operations.Add(new Operation() { Value = lex.Value, Type = EOperationType.VAL });
            }

            GetNewPositionAndLexeme(ref pos, ref lex);

            return true;
        }

        //Операторы
        private bool Operators(ref int pos, ref Lexeme lex)
        {
            if (!Operator(ref pos, ref lex))
            {
                return false;
            }

            while (!(lex is null) && lex.LexType == ELexType.Delimiter)
            {
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!Operator(ref pos, ref lex))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Operator(ref int pos, ref Lexeme lex)
        {
            if (!(lex is null) && lex.LexType == ELexType.Output)
            {
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!Operand(ref pos, ref lex))
                {
                    return false;
                }
                _operations.Add(new Operation() { Value = "OUT", Type = EOperationType.OUT });
            }
            else if (!(lex is null) && lex.LexType == ELexType.Variable)
            {
                _operations.Add(new Operation() { Value = lex.Value, Type = EOperationType.VAR });
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (lex is null || lex.LexType != ELexType.Assignment)
                {
                    ShowError(pos, lex, "Ожидается присвоение");
                    return false;
                }

                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!ArithExpr(ref pos, ref lex))
                {
                    return false;
                }
                _operations.Add(new Operation() { Value = "SET", Type = EOperationType.SET });
            }

            return true;
        }

        private bool ArithExpr(ref int pos, ref Lexeme lex)
        {
            if (!ArithExprHighPriority(ref pos, ref lex))
            {
                return false;
            }

            while (!(lex is null) && (lex.Value.Equals("+") || lex.Value.Equals("-")))
            {
                var prevLex = lex;
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!ArithExprHighPriority(ref pos, ref lex))
                {
                    return false;
                }

                switch (prevLex.Value)
                {
                    case "+":
                        {
                            _operations.Add(new Operation() { Value = "ADD", Type = EOperationType.ADD });
                            break;
                        }
                    case "-":
                        {
                            _operations.Add(new Operation() { Value = "SUB", Type = EOperationType.SUB });
                            break;
                        }
                }
            }

            return true;
        }

        private bool ArithExprHighPriority(ref int pos, ref Lexeme lex)
        {
            if (!Operand(ref pos, ref lex))
            {
                return false;
            }

            while (!(lex is null) && lex.Value.Equals("*") || lex.Value.Equals("/"))
            {
                var prevLex = lex;
                GetNewPositionAndLexeme(ref pos, ref lex);
                if (!Operand(ref pos, ref lex))
                {
                    return false;
                }

                switch (prevLex.Value)
                {
                    case "/":
                        {
                            _operations.Add(new Operation() { Value = "DIV", Type = EOperationType.DIV });
                            break;
                        }
                    case "*":
                        {
                            _operations.Add(new Operation() { Value = "MUL", Type = EOperationType.MUL });
                            break;
                        }
                }
            }

            return true;
        }

        private bool GetNewPositionAndLexeme(ref int pos, ref Lexeme lex)
        {
            pos++;

            if (pos >= _lexemes.Count)
            {
                lex = null;
                return false;
            }
            else
            {
                lex = _lexemes[pos];
                return true;
            }
        }

        private void ShowError(int pos, Lexeme lex, string message)
        {
            string val;

            if (lex is null)
            {
                val = "null";
            }
            else
            {
                val = lex.Value;
            }

            throw new Exception(message + ", Позиция:  " + pos + ", Текущее значение: " + val);
        }
    }
}

namespace LanguageTranslations
{
    class Interpreter
    {
        private List<Operation> _operations;
        private Stack<object> _stack;
        private Dictionary<string, object> _varTable;
        private List<object> _outStream;

        public Interpreter(SyntaxAnalyzer analyzer)
        {
            _operations = analyzer.Operations;
            _stack = new Stack<object>();
            _varTable = new Dictionary<string, object>();
            _outStream = new List<object>();
        }

        private void CheckAndSetVar()
        {
            foreach (var item in _operations)
            {
                if (item.Type == EOperationType.VAR && !_varTable.ContainsKey(item.Value))
                {
                    Console.Write("Укажите значение переменной " + item.Value + ": ");
                    _varTable.Add(item.Value, double.Parse(Console.ReadLine()));
                }
            }
            Console.WriteLine();
        }

        private object PopVal()
        {
            var val = _stack.Pop();

            if (val is Operation)
            {
                return _varTable[((Operation)val).Value];
            }

            return val;
        }

        public void Run()
        {
            if (_operations is null)
            {
                throw new Exception("Invalid operations list");
            }

            CheckAndSetVar();

            int pos = 0;
            int postfixPos = _operations.Count;
            int step = 0;

            Console.WriteLine("Шаг:\t\tОперация:\t\tПеременные:\t\t\tСтек:");

            while (pos < postfixPos)
            {
                Console.Write(step + "\t\t");
                Console.Write(_operations[pos].Value + "\t\t\t");
                foreach (var item in _varTable.Keys)
                {
                    Console.Write(item + ": " + _varTable[item] + " ");
                }
                Console.Write("\t\t\t");
                foreach (var item in _stack)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine();

                switch (_operations[pos].Type)
                {
                    case EOperationType.SET:
                        {
                            var val = PopVal();
                            var var = _stack.Pop();
                            _varTable[((Operation)var).Value] = (double)val;
                            pos++;

                            break;
                        }

                    case EOperationType.ADD:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 + (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.DIV:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 / (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.MUL:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 * (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.SUB:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 - (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.CMPE:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 == (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.CMPLE:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 <= (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.CMPG:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 > (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.CMPGE:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 >= (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.CMPL:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 < (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.CMPNE:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((double)val1 != (double)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.AND:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((bool)val1 && (bool)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.OR:
                        {
                            var val2 = PopVal();
                            var val1 = PopVal();
                            _stack.Push((bool)val1 || (bool)val2);
                            pos++;

                            break;
                        }
                    case EOperationType.NOT:
                        {
                            var val = PopVal();
                            _stack.Push(!(bool)val);
                            pos++;

                            break;
                        }
                    case EOperationType.OUT:
                        {
                            var val = PopVal();
                            _outStream.Add(val);
                            pos++;

                            break;
                        }
                    case EOperationType.JZ:
                        {
                            var adr = PopVal();
                            var val = PopVal();

                            if (!(bool)val)
                            {
                                pos = Convert.ToInt32(adr);
                            }
                            else
                            {
                                pos++;
                            }

                            break;
                        }
                    case EOperationType.JMP:
                        {
                            var adr = PopVal();
                            pos = Convert.ToInt32(adr);

                            break;
                        }
                    case EOperationType.VAL:
                        {
                            _stack.Push(double.Parse(_operations[pos].Value));
                            pos++;

                            break;
                        }
                    case EOperationType.VAR:
                        {
                            _stack.Push(_operations[pos]);
                            pos++;

                            break;
                        }
                }
                step++;
            }

            Console.WriteLine("\nВыходной поток: ");
            foreach (var item in _outStream)
            {
                Console.Write(item + " ");
            }
        }
    }
}