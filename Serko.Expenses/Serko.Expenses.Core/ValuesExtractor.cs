using Serko.Expenses.Core.Exceptions;
using Serko.Expenses.Core.ValueFinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serko.Expenses.Core
{
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

            foreach (var finder in ValueFinders)
            {
                var foundValues = finder.Process(text);

                foreach (var value in foundValues)
                {
                    if (result.ContainsKey(value.Key))
                    {
                        if (!result[value.Key].Equals(value.Value))
                            throw new InvalidInputException($"Double <{value.Key}> tags in your input. Please validate!");

                    }
                    else
                    {
                        result.Add(value.Key, value.Value);
                    }
                }
            }

            return result;
        }
    }
}
