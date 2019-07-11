using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Calculators
{
    interface ICalculator
    {
        decimal Tax { get; }
        string ValueName { get; }
        decimal GetValue(IDictionary<string, string> elements);
        decimal GetGstValue(decimal total);
        decimal GetTotalExcludingGst(decimal total);
    }
}
