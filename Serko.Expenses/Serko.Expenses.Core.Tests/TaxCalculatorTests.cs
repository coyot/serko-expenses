using System.Collections.Generic;
using NUnit.Framework;
using Serko.Expenses.Core.Calculators;
using Serko.Expenses.Core.Exceptions;
using Serko.Expenses.Core.ValueFinders;

namespace Serko.Expenses.Core.Tests
{
    public class TaxCalculatorTests
    {

        TaxCalculator _sut;
        private IDictionary<string, string> notCorrect = new Dictionary<string, string>();
        private IDictionary<string, string> hasTotalNotCorrect = new Dictionary<string, string>() { { "total", "asdf" } };
        private IDictionary<string, string> hasTotalCorrect = new Dictionary<string, string>() { { "total", "110" } };

        [SetUp]
        public void Setup()
        {
            _sut = new TaxCalculator();
        }

        [Test]
        public void Exception_GetValue_NoTotalValue() => Assert.Throws<MissingTotalValueException>(() => _sut.GetValue(notCorrect));
        [Test]
        public void Exception_GetValue_TotalValueIncorrect() => Assert.Throws<InvalidInputException>(() => _sut.GetValue(hasTotalNotCorrect));

        [Test]
        public void GetValue_TotalValueCorrect()
        {
            var result = _sut.GetValue(hasTotalCorrect);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(110));
        }

        [Test]
        public void GetGst_10()
        {
            var vut = 110;
            var resultGst = _sut.GetGstValue(vut);
            var resultTotalNoGst = _sut.GetTotalExcludingGst(vut);


            Assert.That(resultGst, Is.Not.Null);
            Assert.That(resultGst, Is.EqualTo(10));
            Assert.That(resultTotalNoGst, Is.Not.Null);
            Assert.That(resultTotalNoGst, Is.EqualTo(100));
            Assert.That(resultGst + resultTotalNoGst, Is.EqualTo(vut));
        }

        [Test]
        public void GetGst_1000000000()
        {
            var vut = 11000000000;
            var resultGst = _sut.GetGstValue(vut);
            var resultTotalNoGst = _sut.GetTotalExcludingGst(vut);


            Assert.That(resultGst, Is.Not.Null);
            Assert.That(resultGst, Is.EqualTo(1000000000));
            Assert.That(resultTotalNoGst, Is.Not.Null);
            Assert.That(resultTotalNoGst, Is.EqualTo(10000000000));
            Assert.That(resultGst + resultTotalNoGst, Is.EqualTo(vut));
        }

        [Test]
        public void GetGst_0()
        {
            var vut = 0;
            var resultGst = _sut.GetGstValue(vut);
            var resultTotalNoGst = _sut.GetTotalExcludingGst(vut);


            Assert.That(resultGst, Is.Not.Null);
            Assert.That(resultGst, Is.EqualTo(0));
            Assert.That(resultTotalNoGst, Is.Not.Null);
            Assert.That(resultTotalNoGst, Is.EqualTo(0));
            Assert.That(resultGst + resultTotalNoGst, Is.EqualTo(vut));
        }
    }
}