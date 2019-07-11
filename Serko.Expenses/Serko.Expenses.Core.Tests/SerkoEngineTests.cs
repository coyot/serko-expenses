using Moq;
using NUnit.Framework;
using Serko.Expenses.Core.Calculators;
using Serko.Expenses.Core.Exceptions;
using Serko.Expenses.Core.ValueFinders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Tests
{
    public class SerkoEngineTests
    {
        IEngine _sut;
        ICalculator calc;

        [SetUp]
        public void Setup()
        {
            var mockExtractor = new Mock<IValuesExtractor>();
            mockExtractor.Setup(e => e.ExtractValues(It.IsAny<string>())).Returns(new Dictionary<string, string>() {{ "total", "110" }});
            calc = new TaxCalculator();

            _sut = new SerkoEngine(mockExtractor.Object, calc);

        }
        [Test]
        public void ExctractValues_TotalValueSpecified110()
        {
            var values = _sut.ParseAndCalculateGst("the total value is <total>110</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(3));
            Assert.That(values["total"], Is.EqualTo("110"));
            Assert.That(values["gst"], Is.EqualTo("10,00"));
            Assert.That(values["totalNoGst"], Is.EqualTo("100,00"));
        }

        [Test]
        public void ExctractValues_TotalValueSpecified110000()
        {
            var mockExtractor = new Mock<IValuesExtractor>();
            mockExtractor.Setup(e => e.ExtractValues(It.IsAny<string>())).Returns(new Dictionary<string, string>() { { "total", "110000" } });

            _sut = new SerkoEngine(mockExtractor.Object, calc);
            var values = _sut.ParseAndCalculateGst("the total value is <total>110</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(3));
            Assert.That(values["total"], Is.EqualTo("110000"));
            Assert.That(values["gst"], Is.EqualTo("10000,00"));
            Assert.That(values["totalNoGst"], Is.EqualTo("100000,00"));
        }

        [Test]
        public void ExctractValues_TotalValueSpecified321654987()
        {
            var mockExtractor = new Mock<IValuesExtractor>();
            mockExtractor.Setup(e => e.ExtractValues(It.IsAny<string>())).Returns(new Dictionary<string, string>() { { "total", "321654987" } });

            _sut = new SerkoEngine(mockExtractor.Object, calc);
            var values = _sut.ParseAndCalculateGst("the total value is <total>110</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(3));
            Assert.That(values["total"], Is.EqualTo("321654987"));
            Assert.That(values["gst"], Is.EqualTo("29241362,45"));
            Assert.That(values["totalNoGst"], Is.EqualTo("292413624,55"));
        }

        [Test]
        public void ExctractValues_TotalValueSpecified654654_123()
        {
            var mockExtractor = new Mock<IValuesExtractor>();
            mockExtractor.Setup(e => e.ExtractValues(It.IsAny<string>())).Returns(new Dictionary<string, string>() { { "total", "654654,123" } });

            _sut = new SerkoEngine(mockExtractor.Object, calc);
            var values = _sut.ParseAndCalculateGst("the total value is <total>110</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(3));
            Assert.That(values["total"], Is.EqualTo("654654,123"));
            Assert.That(values["gst"], Is.EqualTo("59514,01"));
            Assert.That(values["totalNoGst"], Is.EqualTo("595140,11"));
        }

        [Test]
        public void ExctractValues_TotalValueSpecified654654_123andMoreFinders()
        {
            var finders = new List<IValueFinder>() { new TotalValueFinder(), new VendorValueFinder() };
            var extractor = new ValuesExtractor(finders);

            _sut = new SerkoEngine(extractor, calc);
            var values = _sut.ParseAndCalculateGst("the total value is <total>110</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(3));
            Assert.That(values["total"], Is.EqualTo("110"));
            Assert.That(values["gst"], Is.EqualTo("10,00"));
            Assert.That(values["totalNoGst"], Is.EqualTo("100,00"));
        }

        [Test]
        public void ExctractValues_TotalValueSpecified654654_123andMoreFindersWithProperValues()
        {
            var finders = new List<IValueFinder>() { new TotalValueFinder(), new VendorValueFinder() };
            var extractor = new ValuesExtractor(finders);

            _sut = new SerkoEngine(extractor, calc);
            var values = _sut.ParseAndCalculateGst("the total value is <total>110</total> and vendor <vendor>is vendor</vendor>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(4));
            Assert.That(values["total"], Is.EqualTo("110"));
            Assert.That(values["gst"], Is.EqualTo("10,00"));
            Assert.That(values["totalNoGst"], Is.EqualTo("100,00"));
            Assert.That(values["vendor"], Is.EqualTo("is vendor"));
        }

        [Test]
        public void ExctractValues_ExpenseSubTags()
        {
            var finders = new List<IValueFinder>() { new TotalValueFinder(), new VendorValueFinder(), new ExpenseValueFinder() };
            var extractor = new ValuesExtractor(finders);

            _sut = new SerkoEngine(extractor, calc);
            var values = _sut.ParseAndCalculateGst("<expense><vendor>asdf</vendor></expense><total>1234</total>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Keys.Count, Is.EqualTo(5));
            Assert.That(values["total"], Is.EqualTo("110"));
            Assert.That(values["gst"], Is.EqualTo("10,00"));
            Assert.That(values["totalNoGst"], Is.EqualTo("100,00"));
            Assert.That(values["vendor"], Is.EqualTo("is vendor"));//<expense><vendor>asdf</vendor></expense><total>1234</total>
        }
    }
}
