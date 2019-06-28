using Serko.Expenses.Core.ValueFinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serko.Expenses.Core
{
    class ValuesExtractor
    {
        private IList<IValueFider> Finders { get; set; }

        public ValuesExtractor()
        {

        }

        public IDictionary<string, string> ExtractValues(string text)
        {
            var result = new Dictionary<string, string>();

            foreach (var finder in Finders)
            {
                result.Append(finder.Process(text));
            }

            return result;
        }
    }
}
