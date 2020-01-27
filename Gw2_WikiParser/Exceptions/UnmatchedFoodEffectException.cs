using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Exceptions
{
    public class UnmatchedFoodEffectException : Exception
    {
        public string Line { get; set; }

        public UnmatchedFoodEffectException(string line)
        {
            Line = line;
        }

        public override string ToString()
        {
            return "Unmatched Effect for " + Line + base.ToString();
        }
    }
}
