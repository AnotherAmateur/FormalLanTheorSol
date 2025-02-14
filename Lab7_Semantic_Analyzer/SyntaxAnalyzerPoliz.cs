using Lab5_Lexical_Analyzer;
using Lab5_Lexical_Analyzer.Enums;
using Lab7_Syntax_Analyzer_Poliz.Enums;
using System.Collections.ObjectModel;

namespace Lab7_Syntax_Analyzer_Poliz
{
    public static class SyntaxAnalyzerPoliz
    {
        private static ReadOnlyCollection<Lexeme> _lexemes;
        private static List<PostfixEntry> _poliz;
        private static int _currentPos;
        private const string FOR = "for";
        private const string TO = "to";
        private const string NEXT = "next";
        private const string ASSIGNMENT = "=";
        private const string ADD = "+";
        private const string SUB = "-";

        public static List<PostfixEntry> Poliz { get => _poliz; }

        public static void Parse(List<Lexeme> lexemes)
        {
            _lexemes = new(lexemes);
            _poliz = new();
            _currentPos = 0;
            ParseForLoop();
        }

        private static Lexeme GetLexeme()
        {
            if (_currentPos == _lexemes.Count)
            {
                var lastLex = _lexemes[_currentPos - 1];
                (int, int, int) position = new(lastLex.LinePos, lastLex.LexemePos, lastLex.CharPos);
                ThrowParseException("Отсутствует следующая лексема.", position);
            }

            return _lexemes[_currentPos++];
        }

        private static Lexeme PeekLexeme()
        {
            if (_currentPos == _lexemes.Count)
            {
                var lastLex = _lexemes[_currentPos - 1];
                (int, int, int) position = new(lastLex.LinePos, lastLex.LexemePos, lastLex.CharPos);
                ThrowParseException("Отсутствует следующая лексема.", position);
            }

            return _lexemes[_currentPos];
        }

        private static int WritePoliz(object value, EntryType entryType = EntryType.Cmd, int index = -1)
        {
            if (index == -1)
            {
                _poliz.Add(new PostfixEntry(value, entryType));
            }
            else
            {
                _poliz[index] = new PostfixEntry(value, entryType);
            }

            return _poliz.Count - 1;
        }

        private static void ParseForLoop()
        {
            CheckExpectation(FOR, Categories.Keyword);
            Lexeme identifierFirst = ParseIdentifier();

            CheckExpectation(ASSIGNMENT, Categories.SpecSymb);
            ParseArithmeticExpression();
            WritePoliz(Cmd.SET);

            CheckExpectation(TO, Categories.Keyword);
            int indexForJmp = WritePoliz(identifierFirst.Value, EntryType.Var);

            ParseArithmeticExpression();
            WritePoliz(Cmd.CMPLE);
            int jzIndex = WritePoliz(-1, EntryType.CmdPtr);
            WritePoliz(Cmd.JZ);

            ParseOperators();

            WritePoliz(indexForJmp, EntryType.CmdPtr);
            WritePoliz(Cmd.JMP);
            WritePoliz(_poliz.Count, EntryType.CmdPtr, jzIndex);

            CheckExpectation(NEXT, Categories.Keyword);

            if (_lexemes.Count > _currentPos)
            {
                ThrowParseException("Лексема за пределами цикла FOR-TO-NEXT", PeekLexeme());
            }
        }

        private static void ParseArithmeticExpression()
        {
            ParseOperand();

            // арифмитические вычисления слева направо
            {
                //Lexeme nextLexeme = PeekLexeme(); 
                //if (IsArithmeticOperation(nextLexeme.Value))
                //{
                //    GetLexeme();
                //    ParseArithmeticExpression();
                //    WritePoliz(nextLexeme.Value == ADD ? Cmd.ADD : Cmd.SUB);
                //}
            }

            // арифмитические вычисления справа налево
            while (IsArithmeticOperation(PeekLexeme().Value))
            {
                Lexeme nextLexeme = GetLexeme();
                ParseOperand();
                WritePoliz(nextLexeme.Value == ADD ? Cmd.ADD : Cmd.SUB);
            }
        }

        private static void ParseOperand()
        {
            Lexeme operand = GetLexeme();

            if ((operand.LexCat.Equals(Categories.Identifier) || operand.LexCat.Equals(Categories.Const)) is false)
            {
                ThrowParseException("Ожидается операнд (идентификатор или константа).", operand);
            }

            WritePoliz(operand.Value, operand.LexCat == Categories.Identifier ? EntryType.Var : EntryType.Const);
        }

        private static Lexeme ParseIdentifier()
        {
            Lexeme lexeme = GetLexeme();

            if (lexeme.LexCat.Equals(Categories.Identifier) is false)
            {
                ThrowParseException("Ожидается идентификатор.", lexeme);
            }

            WritePoliz(lexeme.Value, EntryType.Var);

            return lexeme;
        }

        private static void ParseOperators()
        {
            while (PeekLexeme().LexCat.Equals(Categories.Keyword) is false)
            {
                ParseIdentifier();
                CheckExpectation(ASSIGNMENT, Categories.SpecSymb);
                ParseArithmeticExpression();

                WritePoliz(Cmd.SET);
            }
        }

        private static void ParseLogicalExpression()
        {
            ParseOperand();

            Lexeme nextLexeme = PeekLexeme();
            if (IsComparisonOperator(nextLexeme.Value))
            {
                GetLexeme();
                ParseOperand();
            }
        }

        private static void CheckExpectation(string expectedKeyword, Categories category)
        {
            Lexeme lexeme = GetLexeme();

            if ((category.Equals(lexeme.LexCat) && lexeme.Value.Equals(expectedKeyword, StringComparison.Ordinal)) is false)
            {
                ThrowParseException($"Ожидается ключевое слово '{expectedKeyword}'.", lexeme);
            }
        }

        private static bool IsComparisonOperator(string value)
        {
            return value == "<" || value == ">" || value == "<=" || value == ">=" || value == "==" || value == "<>";
        }

        private static bool IsArithmeticOperation(string value)
        {
            return value == ADD || value == SUB;
        }

        private static void ThrowParseException(string message, Lexeme lexeme)
        {
            throw new Exception($"Ошибка в синтаксическом анализе: Позиция: [{lexeme.LinePos}/{lexeme.LexemePos}/{lexeme.CharPos}]. {message} Найдено: {lexeme.LexType}," +
                $" Категория: {lexeme.LexCat}, Значение: {lexeme.Value}");
        }

        private static void ThrowParseException(string message, (int, int, int) position)
        {
            throw new Exception($"Ошибка в синтаксическом анализе: Позиция: [{position.Item1}/{position.Item2}/{position.Item3}]. {message}");
        }
    }

    public struct PostfixEntry
    {
        public object Value { get; }
        public EntryType Type { get; }

        public PostfixEntry(object value, EntryType type)
        {
            Value = value;
            Type = type;
        }
    }
}