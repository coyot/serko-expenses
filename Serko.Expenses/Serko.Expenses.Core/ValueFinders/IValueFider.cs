using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.ValueFinders
{
    interface IValueFider
    {
        IDictionary<string, string> Process(string text);
        bool IsValid(string text);
        bool ShouldProcess(string text);
        bool HasChildren(string text);
        IDictionary<string, string> ExtractValues(string text);
        string ExtractIsland(string text);
        IList<string> GetChildren(string text);
    }
}
