using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Gw2_WikiParser.Extensions
{
    public static class ProcessExtensions
    {
        public static string GetLogMessage(this Process p)
        {
            return $"\"{p.StartInfo.FileName}\" {p.StartInfo.Arguments}";
        }
    }
}
