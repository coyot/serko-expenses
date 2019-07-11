using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core
{
    public interface IEngine
    {
        IDictionary<string, string> ParseAndCalculateGst(string text);
    }
}
