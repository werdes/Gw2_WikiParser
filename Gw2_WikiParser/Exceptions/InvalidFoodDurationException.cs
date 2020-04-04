using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Exceptions
{
    public class InvalidFoodDurationException : Exception
    {
        public string PageTitle { get; set; }

        public InvalidFoodDurationException(string pageTitle)
        {
            PageTitle = pageTitle;
        }

        public InvalidFoodDurationException(string pageTitle, Exception innerException) : base("Invalid Food Duration for " + pageTitle, innerException)
        {
            PageTitle = pageTitle;
        }

        public override string ToString()
        {
            return "Invalid Food Duration for " + PageTitle + base.ToString();
        }
    }
}
