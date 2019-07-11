using Serko.Expenses.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.ValueFinders
{
    public class TotalValueFinder : BaseValueFinder
    {
        public override string TagName => "total";

        public override bool IsValid(string text)
        {
            if (!text.Contains(this.OpeningTag))
                throw new InvalidInputException($"Message does not contain {OpeningTag}! We cannot process this message");

            return base.IsValid(text);
        }

        public override bool ShouldProcess(string text)
        {
            return true;
        }
    }
}
