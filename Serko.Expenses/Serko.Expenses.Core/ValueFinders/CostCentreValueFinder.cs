﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.ValueFinders
{
    public class CostCentreValueFinder : BaseValueFinder
    {
        public override string TagName => "cost_centre";
        public override bool ShouldProcess(string text)
        {
            return true;
        }

        public override bool IsValid(string text)
        {
            if (!text.Contains(OpeningTag) && !text.Contains(ClosingTag))
                return true;

            return base.IsValid(text);
        }

        public override IDictionary<string, string> ExtractValues(string text)
        {
            if (!text.Contains(OpeningTag) && !text.Contains(ClosingTag))
                return new Dictionary<string, string>() { { TagName, "UNKNOWN" } };

            return base.ExtractValues(text);
        }
    }
}
