using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5_Lexical_Analyzer.Enums
{
    public enum States
    {
        Start,
        Final,
        Const,
        Delimiter,
        Word,
        Arithmetic,
        Assignment,
        ComparisonRight,
        ComparisonLeft,
        Comparison,
        Error
    }
}