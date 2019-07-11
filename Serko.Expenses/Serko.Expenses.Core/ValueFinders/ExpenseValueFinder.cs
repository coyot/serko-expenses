using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serko.Expenses.Core.ValueFinders
{
    public class ExpenseValueFinder : BaseValueFinder
    {
        public override string TagName => "expense";
        public override IEnumerable<IValueFinder> ValueFinders {
            get => new List<IValueFinder>() {
                new CostCentreValueFinder(),
                new DateValueFinder(),
                new DescriptionValueFinder(),
                new PaymentMethodValueFinder(),
                new VendorValueFinder()
            };
        set => base.ValueFinders = value; }
    }
}
