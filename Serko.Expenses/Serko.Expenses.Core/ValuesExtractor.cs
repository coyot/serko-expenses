using Serko.Expenses.Core.Exceptions;
using Serko.Expenses.Core.ValueFinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serko.Expenses.Core
{
    /// <summary>
    /// Extract key-value pairs from text based on configured tags
    /// </summary>
    public class ValuesExtractor : IValuesExtractor
    {
        private IEnumerable<IValueFinder> ValueFinders { get; set; }
        private IEnumerable<IComplexValueFinder> ValueComplexFinders { get; set; }

        public ValuesExtractor(IEnumerable<IValueFinder> valueFinders, IEnumerable<IComplexValueFinder> complexFinders)
        {
            ValueFinders = valueFinders;
            ValueComplexFinders = complexFinders;
        }

        public IDictionary<string, string> ExtractValues(string text)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(text))
                return result;

            ProcessSimple(text, result);
            ProcessComplex(text, result);

            return result;
        }

        private IDictionary<string, string> ProcessSimple(string text, IDictionary<string, string> result)
        {
            foreach (var finder in ValueFinders)
            {
                var foundValues = finder.Process(text);
                result = TryMerge(result, foundValues);
            }

            return result;
        }

        private IDictionary<string, string> ProcessComplex(string text, IDictionary<string, string> result)
        {
            foreach (var finder in ValueComplexFinders)
            {
                var foundValues = finder.Process(text);
                result = TryMerge(result, foundValues);
            }

            return result;
        }

        private IDictionary<string, string> TryMerge(IDictionary<string, string> to, IDictionary<string, string> from)
        {
            foreach (var value in from)
            {
                if (to.ContainsKey(value.Key))
                {
                    if (!to[value.Key].Equals(value.Value))
                        throw new InvalidInputException($"Double <{value.Key}> tags in your input. Please validate!");

                }
                else
                {
                    to.Add(value.Key, value.Value);
                }
            }

            return to;
        }

    }
}
