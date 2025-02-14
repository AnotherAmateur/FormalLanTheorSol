using Lab5_Lexical_Analyzer;
using Lab5_Lexical_Analyzer.Enums;
using System.Collections.ObjectModel;

namespace Lab6_Syntax_Analyzer
{
    public static class SyntaxAnalyzer
    {
        private static ReadOnlyCollection<Lexeme> _lexemes;
        private static int currentPos;
        private const string FOR = "for";
        private const string TO = "to";
        private const string NEXT = "next";
        private const string ASSIGNMENT = "=";

        public static string Parse(List<Lexeme> lexemes)
        {
            _lexemes = new(lexemes);
            currentPos = 0;

            try
            {
                ParseForLoop();
                return "Синтаксический анализ завершен успешно.";
            }
            catch (Exception ex)
            {
                return $"Ошибка в синтаксическом анализе: {ex.Message}";
            }
        }

        private static Lexeme GetLexeme()
        {
            if (currentPos == _lexemes.Count)
            {
                ThrowParseException("Отсутствует следующея необходимая лексема", _lexemes[currentPos - 1]);
            }

            return _lexemes[currentPos++];
        }

        private static Lexeme PeekLexeme()
        {
            if (currentPos == _lexemes.Count)
            {
                ThrowParseException("Отсутствует следующея необходимая лексема", _lexemes[currentPos - 1]);
            }

            return _lexemes[currentPos];
        }

        private static void ParseForLoop()
        {
            CheckExpectation(FOR, Categories.Keyword);
            ParseIdentifier();

            CheckExpectation(ASSIGNMENT, Categories.SpecSymb);
            ParseArithmeticExpression();

            CheckExpectation(TO, Categories.Keyword);
            ParseArithmeticExpression();

            ParseOperators();
            CheckExpectation(NEXT, Categories.Keyword);

            if (_lexemes.Count > currentPos)
            {
                ThrowParseException("Лексема за пределами цикла", PeekLexeme());
            }
        }

        private static void ParseArithmeticExpression()
        {
            ParseOperand();

            Lexeme nextLexeme = PeekLexeme();

            if (IsArithmeticOperation(nextLexeme.Value))
            {
                GetLexeme();
                ParseArithmeticExpression();
            }
        }

        private static void ParseOperand()
        {
            Lexeme operand = GetLexeme();

            if ((operand.LexCat.Equals(Categories.Identifier) || operand.LexCat.Equals(Categories.Const)) is false)
            {
                ThrowParseException("Ожидается операнд (идентификатор или константа).", operand);
            }
        }

        private static void ParseIdentifier() 
        {
            Lexeme lexeme = GetLexeme();

            if (lexeme.LexCat.Equals(Categories.Identifier) is false)
            {
                ThrowParseException("Ожидается идентификатор.", lexeme);
            }
        }

        private static void ParseOperators()
        {
            while (PeekLexeme().LexCat.Equals(Categories.Keyword) is false)
            {
                ParseIdentifier();
                CheckExpectation(ASSIGNMENT, Categories.SpecSymb);
                ParseArithmeticExpression();
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

            if ((category.Equals(lexeme.LexCat) && lexeme.Value.Equals(expectedKeyword, StringComparison.OrdinalIgnoreCase)) is false)
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
            return value == "+" || value == "-";
        }

        private static void ThrowParseException(string message, Lexeme lexeme)
        {
            throw new Exception($"Позиция: [{lexeme.LinePos}/{lexeme.LexemePos}/{lexeme.CharPosAbsolute}]. {message} Найдено: {lexeme.LexType}," +
                $" Категория: {lexeme.LexCat}, Значение: {lexeme.Value}");
        }
    }
}
