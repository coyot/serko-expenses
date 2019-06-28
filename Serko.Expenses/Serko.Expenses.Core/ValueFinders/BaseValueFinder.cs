﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.ValueFinders
{
    /// <summary>
    /// Base class for text operation - main idea is to find the "island" of xml
    /// value within the text.
    /// </summary>
    abstract class BaseValueFinder : IValueFider
    {
        public abstract string TagName { get; set; }

        private string OpeningTag => $"<{TagName}>";
        private string ClosingTag => $"</{TagName}>";


        public bool IsValid(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            if (text.Contains(OpeningTag))
            {
                return text.Contains(ClosingTag);
            }

            return true;
        }

        public IDictionary<string, string> ExtractValues(string text)
        {
            if (!IsValid(text))
                return null;

            return null;
        }

        public string ExtractIsland(string text)
        {
            return string.Empty;
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
            throw new NotImplementedException();
        }
    }
}
