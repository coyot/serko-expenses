using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serko.Expenses.Core.ValueFinders
{
    public class ExpenseValueFinder : BaseValueFinder, IComplexValueFinder
    {
        public override string TagName => "expense";
        public ExpenseValueFinder(IEnumerable<IValueFinder> finders)
        {
            ValueFinders = finders;
        }
    }
}
