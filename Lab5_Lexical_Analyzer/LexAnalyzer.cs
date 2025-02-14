using Lab5_Lexical_Analyzer.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Lab5_Lexical_Analyzer
{
    public static class LexAnalyzer
    {
        public static Lexeme ErrorInfo { get; private set; }

        public static (bool, List<Lexeme>) Analyze(string codeStr)
        {
            States curState = States.Start;
            States prevState;

            var lexemes = new List<Lexeme>();
            var lexemeCur = new StringBuilder();
            var lexemeNext = new StringBuilder();

            bool isLexEnd;

            int textPos = 0;
            int charPosAbsolute = -1;
            int linePos = 0;
            int lexemePos = 0;

            int lexemePrevLen = 0;
           
            while (curState != States.Final && curState != States.Error)
            {
                prevState = curState;
                isLexEnd = true;
                char symbol = default;

                if (textPos == codeStr.Length)
                {
                    curState = States.Final;
                }
                else
                {
                    symbol = codeStr[textPos];
                }

                switch (curState)
                {
                    case States.Start:
                        {
                            isLexEnd = false;

                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                break;
                            }
                            else if (char.IsDigit(symbol))
                            {
                                curState = States.Const;
                            }
                            else if (char.IsLetter(symbol))
                            {
                                curState = States.Word;
                            }
                            else if (symbol == '>')
                            {
                                curState = States.ComparisonRight;
                            }
                            else if (symbol == '<')
                            {
                                curState = States.ComparisonLeft;
                            }
                            else if (symbol == '+' || symbol == '-')
                            {
                                curState = States.Arithmetic;
                            }
                            else if (symbol == '=')
                            {
                                curState = States.Assignment;
                            }
                            else if (symbol == ';')
                            {
                                curState = States.Delimiter;
                            }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                                break;
                            }

                            lexemeCur.Append(symbol);
                            break;
                        }
                    case States.Word:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                curState = States.Start;
                                break;
                            }
                            else if (char.IsLetterOrDigit(symbol))
                            {
                                lexemeCur.Append(symbol);
                                isLexEnd = false;
                            }
                            else if (symbol == ';')
                            {
                                curState = States.Delimiter;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '>')
                            {
                                curState = States.ComparisonRight;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '<')
                            {
                                curState = States.ComparisonLeft;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '=')
                            {
                                curState = States.Assignment;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '+' || symbol == '-')
                            {
                                curState = States.Arithmetic;
                                lexemeNext.Append(symbol);
                            }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                            }

                            break;
                        }
                    case States.Const:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                curState = States.Start;
                            }
                            else if (char.IsDigit(symbol))
                            {
                                lexemeCur.Append(symbol);
                                isLexEnd = false;
                            }
                            else if (symbol == ';')
                            {
                                curState = States.Delimiter;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '>')
                            {
                                curState = States.ComparisonRight;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '<')
                            {
                                curState = States.ComparisonLeft;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '=')
                            {
                                curState = States.Assignment;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '+' || symbol == '-')
                            {
                                curState = States.Arithmetic;
                                lexemeNext.Append(symbol);
                            }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                            }

                            break;
                        }
                    case States.Comparison:
                    case States.ComparisonRight:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                curState = States.Start;
                            }
                            else if (char.IsDigit(symbol))
                            {
                                curState = States.Const;
                                lexemeNext.Append(symbol);
                            }
                            else if (char.IsLetter(symbol))
                            {
                                curState = States.Word;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == ';')
                            {
                                curState = States.Delimiter;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '=')
                            {
                                curState = States.Start;
                                lexemeCur.Append(symbol);
                            }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                            }

                            break;
                        }
                    case States.ComparisonLeft:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                curState = States.Start;
                            }
                            else if (symbol == ';')
                            {
                                curState = States.Delimiter;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '>')
                            {
                                curState = States.Start;
                                lexemeCur.Append(symbol);
                            }
                            else if (symbol == '=')
                            {
                                curState = States.Start;
                                lexemeCur.Append(symbol);
                            }
                            else if (char.IsLetter(symbol))
                            {
                                curState = States.Word;
                                lexemeNext.Append(symbol);
                            }
                            else if (char.IsDigit(symbol))
                            {
                                curState = States.Const;
                                lexemeNext.Append(symbol);
                            }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                            }

                            break;
                        }
                    case States.Arithmetic:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                curState = States.Start;
                            }
                            else if (char.IsLetter(symbol))
                            {
                                curState = States.Word;
                                lexemeNext.Append(symbol);
                            }
                            else if (char.IsDigit(symbol))
                            {
                                curState = States.Const;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == ';')
                            {
                                curState = States.Delimiter;
                                lexemeNext.Append(symbol);
                            }
                            else if (symbol == '+' || symbol == '-')
                            {
                                lexemeNext.Append(symbol);
                            }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                            }

                            break;
                        }
                    case States.Assignment:
                        {
                            if (symbol == '=')
                            {
                                lexemeCur.Append(symbol);
                            }
                            else
                            {
                                lexemeNext.Append(symbol);
                            }

                            curState = States.Start;
                            break;
                        }
                    case States.Delimiter:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                if (symbol == '\n')
                                {
                                    lexemePos = 0;
                                    ++linePos;
                                }

                                curState = States.Start;
                                break;
                            }
                            else if (char.IsDigit(symbol))
                            {
                                curState = States.Const;
                            }
                            else if (char.IsLetter(symbol))
                            {
                                curState = States.Word;
                            }
                            else if (symbol == '>')
                            {
                                curState = States.ComparisonRight;
                            }
                            else if (symbol == '<')
                            {
                                curState = States.ComparisonLeft;
                            }
                            else if (symbol == '+' || symbol == '-')
                            {
                                curState = States.Arithmetic;
                            }
                            else if (symbol == '=')
                            {
                                curState = States.Assignment;
                            }
                            else if (symbol == ';') { }
                            else
                            {
                                HandleError(prevState, lexemeCur.ToString().Trim() + symbol, linePos, lexemePos++, charPosAbsolute);
                                break;
                            }

                            lexemeNext.Append(symbol);
                            break;
                        }
                    default:
                        break;
                }

                if (isLexEnd && lexemeCur.Length > 0)
                {
                    string lexeme = lexemeCur.ToString().Trim();
                    charPosAbsolute = charPosAbsolute < 0 ? textPos - lexeme.Length : charPosAbsolute + lexemePrevLen;
                    AddLexem(prevState, lexeme, lexemes, linePos, lexemePos++, charPosAbsolute);
                    lexemeCur = lexemeNext;
                    lexemeNext = new();
                    lexemePrevLen = lexeme.Length;
                }

                textPos++;
            }

            void HandleError(States state, string value, int linePos, int lexemePos, int charPosNoWhiteSpace)
            {
                curState = States.Error;
                isLexEnd = false;
                ErrorInfo = new(default, default, value, linePos, lexemePos, charPosNoWhiteSpace);
            }


            return (curState == States.Final, lexemes);
        }

        private static void AddLexem(States state, string value, List<Lexeme> lexems, int linePos, int lexemePos, int charPosNoWhiteSpace)
        {
            LexTypes lexType = default;
            Categories lexCat = default;

            switch (state)
            {
                case States.Const:
                    lexType = LexTypes.Const;
                    lexCat = Categories.Const;
                    break;
                case States.Delimiter:
                    lexType = LexTypes.Delimiter;
                    lexCat = Categories.SpecSymb;
                    break;
                case States.Word:
                    switch (value)
                    {
                        case "for":
                            lexType = LexTypes.For;
                            lexCat = Categories.Keyword;
                            break;
                        case "next":
                            lexType = LexTypes.Next;
                            lexCat = Categories.Keyword;
                            break;
                        case "to":
                            lexType = LexTypes.To;
                            lexCat = Categories.Keyword;
                            break;
                        default:
                            lexType = LexTypes.Var;
                            lexCat = Categories.Identifier;
                            break;
                    }
                    break;
                case States.Arithmetic:
                    lexType = LexTypes.Arithmetic;
                    lexCat = Categories.SpecSymb;
                    break;
                case States.Assignment:
                    lexCat = Categories.SpecSymb;
                    lexType = value == "==" ? LexTypes.Relation : LexTypes.Assignment;
                    break;
                case States.Comparison:
                case States.ComparisonRight:
                case States.ComparisonLeft:
                    lexType = LexTypes.Relation;
                    lexCat = Categories.SpecSymb;
                    break;
            }

            lexems.Add(new Lexeme(lexType, lexCat, value, linePos, lexemePos, charPosNoWhiteSpace));
        }
    }
}