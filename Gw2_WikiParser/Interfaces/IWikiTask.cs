using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gw2_WikiParser.Interfaces
{
    interface IWikiTask
    {
        Task<bool> Run();
        string GetProperties();
    }
}
