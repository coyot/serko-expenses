﻿using Serko.Expenses.Core.Exceptions;
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

        private string OpeningTag => $"<{TagName}>";
        private string ClosingTag => $"</{TagName}>";
        private IList<IValueFinder> _valueFinders;

        public BaseValueFinder()
        {
            _valueFinders = new List<IValueFinder>();
        }

        public BaseValueFinder(IList<IValueFinder> finders)
        {
            _valueFinders = finders;
        }

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
            foreach (var finder in _valueFinders)
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

        public string ExtractIsland(string text)
        {
            var openIndex = text.IndexOf(OpeningTag);
            var closeIndex = text.IndexOf(ClosingTag);

            return text.Substring(openIndex, closeIndex  - openIndex + ClosingTag.Length);
        }

        public bool HasChildren(string island)
        {
            return _valueFinders.Any(finder => finder.ShouldProcess(island));
        }

        public IList<string> GetChildren(string island)
        {
            if (!HasChildren(island))
                return new List<string>(0);

            var result = new List<string>();

            foreach(var finder in _valueFinders)
            {
                if (finder.ShouldProcess(island))
                    result.Add(finder.ExtractIsland(island));
            }

            return result;
        }

        public IDictionary<string, string> Process(string text)
        {
            if (!ShouldProcess(text))
                return new Dictionary<string, string>();

            if (!IsValid(text))
                return new Dictionary<string, string>();

            return ExtractValues(text);
        }

        public bool ShouldProcess(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return text.Contains(OpeningTag) || text.Contains(ClosingTag);
        }
    }
}
