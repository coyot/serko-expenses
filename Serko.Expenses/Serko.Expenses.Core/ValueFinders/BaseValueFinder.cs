using Serko.Expenses.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Serko.Expenses.Core.ValueFinders
{
    /// <summary>
    /// Base class for text operation - main idea is to find the "island" of xml
    /// value within the text.
    /// </summary>
    public abstract class BaseValueFinder : IValueFider
    {
        public abstract string TagName { get; }

        private string OpeningTag => $"<{TagName}>";
        private string ClosingTag => $"</{TagName}>";


        public bool IsValid(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            var regexOpen = new Regex(OpeningTag);
            var regexClose = new Regex(ClosingTag);

            if (regexOpen.Matches(text).Count > 1)
                throw new InvalidInputException("Illegal state - too many opening tags");

            if (regexClose.Matches(text).Count > 1)
                throw new InvalidInputException("Illegal state - too many closing tags");

            if (text.Contains(OpeningTag))
            {
                return text.Contains(ClosingTag);
            }

            return false;
        }

        public IDictionary<string, string> ExtractValues(string text)
        {
            if (!IsValid(text))
                return null;

            var result = new Dictionary<string, string>();
            var island = this.ExtractIsland(text);
            if (!HasChildren(island))
            {
                var openLength = OpeningTag.Length;
                var closeLength = ClosingTag.Length;
                var substring = island.Substring(openLength, island.Length - openLength - closeLength);
                result.Add(TagName, substring);

                return result;

            }
            return null;
        }

        public string ExtractIsland(string text)
        {
            if(text.StartsWith(OpeningTag) && text.EndsWith(ClosingTag))
                return text;

            var openIndex = text.IndexOf(OpeningTag);
            var closeIndex = text.IndexOf(ClosingTag);

            return text.Substring(openIndex, closeIndex + ClosingTag.Length - openIndex);
        }

        public bool HasChildren(string text)
        {
            return false;
        }

        public IList<string> GetChildren(string text)
        {
            if (!HasChildren(text))
                return null;

            return null;
        }

        public IDictionary<string, string> Process(string text)
        {
            if (!ShouldProcess(OpeningTag))
                return null;

            if (!IsValid(OpeningTag))
                return null;

            return null;
        }

        public bool ShouldProcess(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return text.Contains(OpeningTag) || text.Contains(ClosingTag);
        }
    }
}
