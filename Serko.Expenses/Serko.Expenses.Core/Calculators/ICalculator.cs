using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Calculators
{
    interface ICalculator
    {
        string ValueName { get; set; }
        decimal GetValue(IDictionary<string, string> elements);
        decimal GetGskValue(decimal total);
    }
}
