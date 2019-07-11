using NUnit.Framework;
using Serko.Expenses.Core.Exceptions;
using Serko.Expenses.Core.ValueFinders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Tests
{
    public class ValuesExtractorTests
    {
        ValuesExtractor _sut;

        [SetUp]
        public void Setup()
        {
            var subfinders = new List<IValueFinder>() { new TotalValueFinder() };
            _sut = new ValuesExtractor(subfinders);

        }
        [Test]
        public void Exception_NoTotalTag() => Assert.Throws<InvalidInputException>(() => _sut.ExtractValues("<tag>asdf</tag>"));
        [Test]
        public void Exception_OnlyOpenTotalTag() => Assert.Throws<InvalidInputException>(() => _sut.ExtractValues("asdf<total>asdf"));
        [Test]
        public void ExctractValues_TotalValueSpecified110()
        {
            var values = _sut.ExtractValues("the total value is <total>110</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values["total"], Is.EqualTo("110"));
        }
        [Test]
        public void ExctractValues_TotalValueSpecified110AndOtherUnknown()
        {
            var values = _sut.ExtractValues("the total value is <total>110</total> and <otherTag>asdf</otherTag>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values["total"], Is.EqualTo("110"));
        }
    }
}
