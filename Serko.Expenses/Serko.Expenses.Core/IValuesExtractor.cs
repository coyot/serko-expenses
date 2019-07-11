using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core
{
    public interface IValuesExtractor
    {
        IDictionary<string, string> ExtractValues(string text);
    }
}
