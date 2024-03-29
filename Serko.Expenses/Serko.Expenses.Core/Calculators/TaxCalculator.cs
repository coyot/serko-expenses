﻿using Serko.Expenses.Core.Exceptions;
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
            return (total / (1+Tax))*Tax;
        }

        public decimal GetTotalExcludingGst(decimal total)
        {
            var gst = GetGstValue(total);

            return total - gst;
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
