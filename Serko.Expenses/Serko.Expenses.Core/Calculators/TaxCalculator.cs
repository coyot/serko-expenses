using Serko.Expenses.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Calculators
{
    public class TaxCalculator : ICalculator
    {
        public decimal Tax => 0.1m;

        public string ValueName { get => "total"; }

        public decimal GetGstValue(decimal total)
        {
            throw new NotImplementedException();
        }

        public decimal GetValue(IDictionary<string, string> elements)
        {
            if (elements.ContainsKey(ValueName))
            {
                var notParsed = elements[ValueName];
                decimal result;

                if (!decimal.TryParse(notParsed, out result))
                    throw new InvalidInputException($"Value <{ValueName}> in tag is not correct!");

                return result;
            }

            throw new MissingTotalValueException($"No <{ValueName}> tag specified!");
        }
    }
}
