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
        private IList<IValueFinder> ValueFinders { get; set; }

        public ValuesExtractor(IEnumerable<IValueFinder> valueFinders)
        {
            ValueFinders = valueFinders.ToList();
        }

        public IDictionary<string, string> ExtractValues(string text)
        {
            var result = new Dictionary<string, string>();

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
