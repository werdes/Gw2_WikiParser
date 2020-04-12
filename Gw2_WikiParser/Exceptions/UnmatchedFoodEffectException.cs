using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Exceptions
{
    public class UnmatchedFoodEffectException : Exception
    {
        public string Line { get; set; }

        public UnmatchedFoodEffectException(string line, string message = "")
            : base("Unmatched Effect for " + line + ", " + message)
        {
            Line = line;
        }

    }
}
