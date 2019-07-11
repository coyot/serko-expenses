using Serko.Expenses.Core.Calculators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core
{
    /// <summary>
    /// Main engine. Two purposes
    ///  - extract values
    ///  - calculate gst and total-gst and add to the resulted kvp result
    /// </summary>
    public class SerkoEngine : IEngine
    {
        IValuesExtractor ValuesExtractor { get; set; }
        ICalculator Calculator { get; set; }

        public SerkoEngine(IValuesExtractor extractor, ICalculator calculator)
        {
            ValuesExtractor = extractor;
            Calculator = calculator;
        }

        public IDictionary<string, string> ParseAndCalculateGst(string text)
        {
            var result = ValuesExtractor.ExtractValues(text);

            var total = Calculator.GetValue(result);
            var gst = Calculator.GetGstValue(total);
            result.Add("gst", gst.ToString("F"));

            var totalNoGst = Calculator.GetTotalExcludingGst(total);
            result.Add("totalNoGst", totalNoGst.ToString("F"));

            return result;
        }
    }
}
