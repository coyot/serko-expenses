using Serko.Expenses.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Serko.Expenses.Core.ValueFinders
{
    /// <summary>
    /// Base class for text operation - main idea is to find the "island" of xml
    /// value within the text.
    /// </summary>
    public abstract class BaseValueFinder : IValueFinder
    {
        public abstract string TagName { get; }

        public string OpeningTag => $"<{TagName}>";
        public string ClosingTag => $"</{TagName}>";
        public virtual IEnumerable<IValueFinder> ValueFinders { get; set; }

        public BaseValueFinder()
        {
            this.ValueFinders = new List<IValueFinder>();
        }

        public BaseValueFinder(IEnumerable<IValueFinder> finders)
        {
            this.ValueFinders = finders;
        }

        public IDictionary<string, string> Process(string text)
        {
            if (!ShouldProcess(text))
                return new Dictionary<string, string>();

            if (!IsValid(text))
                return new Dictionary<string, string>();

            return ExtractValues(text);
        }

        public virtual bool ShouldProcess(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return text.Contains(OpeningTag) || text.Contains(ClosingTag);
        }

        public virtual IDictionary<string, string> ExtractValues(string text)
        {
            var result = new Dictionary<string, string>();

            if (!IsValid(text))
                return result;

            var island = this.ExtractIsland(text);
            var insideIsland = island.Substring(OpeningTag.Length, island.Length - ClosingTag.Length - OpeningTag.Length);
            if (!HasChildren(island))
            {
                result.Add(TagName, insideIsland);

                return result;

            }
            foreach (var finder in ValueFinders)
            {
                var foundValues = finder.ExtractValues(insideIsland);

                foreach (var value in foundValues)
                {
                    if (result.ContainsKey(value.Key))
                        throw new InvalidInputException("Double tags in your input. Please validate!");

                    result.Add(value.Key, value.Value);
                }
            }

            return result;
        }

        public virtual bool IsValid(string text)
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
                if(!text.Contains(ClosingTag))
                    throw new InvalidInputException($"Closing tag for {OpeningTag} not found!");

                return true;
            }

            return false;
        }


        public string ExtractIsland(string text)
        {
            var openIndex = text.IndexOf(OpeningTag);
            var closeIndex = text.IndexOf(ClosingTag);

            return text.Substring(openIndex, closeIndex  - openIndex + ClosingTag.Length);
        }

        public bool HasChildren(string island)
        {
            return ValueFinders.Any(finder => finder.ShouldProcess(island));
        }

        public IList<string> GetChildren(string island)
        {
            if (!HasChildren(island))
                return new List<string>(0);

            var result = new List<string>();

            foreach(var finder in ValueFinders)
            {
                if (finder.ShouldProcess(island))
                    result.Add(finder.ExtractIsland(island));
            }

            return result;
        }
    }
}
