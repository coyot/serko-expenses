using System.Collections.Generic;
using NUnit.Framework;
using Serko.Expenses.Core.Exceptions;
using Serko.Expenses.Core.ValueFinders;

namespace Serko.Expenses.Core.Tests
{
    public class BaseValueFinderTests
    {
        private class TagBaseValueFinderImpl : BaseValueFinder
        {
            public TagBaseValueFinderImpl(IList<IValueFinder> finders) : base(finders)
            {
            }

            public override string TagName { get => "tag"; }
        }
        private class Tag2BaseValueFinderImpl : BaseValueFinder
        {
            public override string TagName { get => "tag2"; }
        }
        private class Tag3BaseValueFinderImpl : BaseValueFinder
        {
            public override string TagName { get => "tag3"; }
        }

        TagBaseValueFinderImpl _sut;
        private const string correctSimpleString = "<tag>test</tag>";
        private const string correctString = "asdf<tag>test</tag>asdf";
        private const string correctComplexString = "<tag>test</tag>asdf<tag2>asdf</tag2>";
        private const string notCorrectComplexTwiceClosedString = "asdf</tag>test</tag>asdf<tag2>asdf</tag2>";
        private const string notCorrectComplexTwiceOpenedString = "asdf<tag>test<tag>asdf<tag2>asdf</tag2>";
        private const string complexWithChildren = "<tag><tag2>test2</tag2><tag3>test3</tag3></tag>";

        [SetUp]
        public void Setup()
        {
            var subfinders = new List<IValueFinder>() { new Tag2BaseValueFinderImpl(), new Tag3BaseValueFinderImpl() };
            _sut = new TagBaseValueFinderImpl(subfinders);
        }

        [Test]
        public void NotShouldProcess_EmptyString() => Assert.That(_sut.ShouldProcess(string.Empty), Is.False);
        [Test]
        public void NotShouldProcess_NullString() => Assert.That(_sut.ShouldProcess(null), Is.False);
        [Test]
        public void ShouldProcess_CorrectString() => Assert.That(_sut.ShouldProcess(correctString), Is.True);
        [Test]
        public void ShouldProcess_CorrectComplexString() => Assert.That(_sut.ShouldProcess(correctComplexString), Is.True);
        [Test]
        public void ShouldProcess_IncorrectComplexString() => Assert.That(_sut.ShouldProcess(notCorrectComplexTwiceClosedString), Is.True);
        [Test]
        public void NotValid_EmptyString() => Assert.That(_sut.IsValid(string.Empty), Is.False);
        [Test]
        public void NotValid_NullString() => Assert.That(_sut.IsValid(null), Is.False);
        [Test]
        public void Exception_DuplicateOpenings() => Assert.Throws<InvalidInputException>(() => _sut.IsValid("<tag><tag>"));
        [Test]
        public void Exception_DuplicateClosings() => Assert.Throws<InvalidInputException>(() => _sut.IsValid("</tag></tag>"));

        [Test]
        public void Valid_OpenedAndClosed() => Assert.That(_sut.IsValid(correctSimpleString), Is.True);
        [Test]
        public void Valid_OpenedAndClosedWithText() => Assert.That(_sut.IsValid(correctString), Is.True);
        [Test]
        public void Valid_OpenedAndClosedComplexAndMoreValidTags() => Assert.That(_sut.IsValid(correctComplexString), Is.True);
        [Test]
        public void Valid_OpenedAndClosedComplexAndMoreNotValidTags() => Assert.That(_sut.IsValid("asdf<tag>test</tag>asdf<tag2>asdf<tag2>"), Is.True);
        [Test]
        public void Valid_ComplexWithChildren () => Assert.That(_sut.IsValid(complexWithChildren), Is.True);
        [Test]
        public void Exception_Valid_OpenedComplex() => Assert.Throws<InvalidInputException>(() => _sut.IsValid("asdf<tag>testasdf"));
        [Test]
        public void NotValid_ClosedComplex() => Assert.That(_sut.IsValid("asdf</tag>testasdf"), Is.False);
        [Test]
        public void Exception_NotValid_OpenedComplexAndMoreValidTags() => Assert.Throws<InvalidInputException>(() => _sut.IsValid("asdf<tag>testasdf<tag2>asdf</tag2>"));
        [Test]
        public void Exception_NotValid_OpenedComplexAndMoreNotValidTags() => Assert.Throws<InvalidInputException>(() => _sut.IsValid("asdf<tag>testasdf<tag2>asdf<tag2>"));
        [Test]
        public void Exception_TwiceClosedComplexAndMoreNotValidTags() => Assert.Throws<InvalidInputException>(() => _sut.IsValid(notCorrectComplexTwiceClosedString));
        [Test]
        public void Exception_TwiceOpenedComplexAndMoreNotValidTags() => Assert.Throws<InvalidInputException>(() => _sut.IsValid(notCorrectComplexTwiceOpenedString));
        [Test]
        public void ExctractValues_EmptyForEmptyString() => Assert.That(_sut.ExtractValues(string.Empty).Count, Is.EqualTo(0));

        [Test]
        public void ExctractValues_NotEmptyForOpenedAndClosedSimple()
        {
            var values = _sut.ExtractValues(correctSimpleString);

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.ContainsKey(_sut.TagName), Is.True);
            Assert.That(values[_sut.TagName], Is.EqualTo("test"));
        }
        [Test]
        public void ExctractValues_NotEmptyForOpenedAndClosedComplex()
        {
            var values = _sut.ExtractValues(correctComplexString);

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.ContainsKey(_sut.TagName), Is.True);
            Assert.That(values[_sut.TagName], Is.EqualTo("test"));
        }
        [Test]
        public void Exception_ExctractValues_NotEmptyForOpenedAndClosedWithDuplicateChildren() => Assert.Throws<InvalidInputException>(() => _sut.ExtractValues("<tag><tag2>test2</tag2><tag2>test3</tag2></tag>"));

        [Test]
        public void ExctractValues_NotEmptyForOpenedAndClosedWithChildren()
        {
            var values = _sut.ExtractValues("<tag><tag2>test2</tag2><tag3>test3</tag3></tag>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values["tag2"], Is.EqualTo("test2"));
            Assert.That(values["tag3"], Is.EqualTo("test3"));
        }

        [Test]
        public void ExctractValues_NotEmptyForOpenedAndClosedAsChild()
        {
            var values = _sut.ExtractValues("<tag2><tag>test</tag></tag2>");

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values["tag"], Is.EqualTo("test"));
        }
        [Test]
        public void ExtractIsland_NotEmptyForOpenedAndClosedWithChildren()
        {
            var value = _sut.ExtractIsland(complexWithChildren);

            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo(complexWithChildren));
        }

        [Test]
        public void ExtractIsland_NotEmptyForOpenedAndClosedWithChildrenComplex()
        {
            var value = _sut.ExtractIsland($"asdf{complexWithChildren}asdf2");

            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo(complexWithChildren));
        }

        [Test]
        public void ExtractIsland_NotEmptyForOpenedAndClosedSimple()
        {
            var value = _sut.ExtractIsland(correctSimpleString);

            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo(correctSimpleString));
        }

        [Test]
        public void ExtractIsland_NotEmptyForOpenedAndClosedComplex()
        {
            var value = _sut.ExtractIsland(correctComplexString);

            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo("<tag>test</tag>"));
        }

        [Test]
        public void Children_NoChildrenForOpenedAndClosed()
        {
            var value = _sut.HasChildren(correctString);
            var children = _sut.GetChildren(correctString);

            Assert.That(value, Is.False);
            Assert.That(children, Is.Empty);
        }

        [Test]
        public void Children_1ChildForOpenedAndClosedComplex()
        {
            var stringToTest = "<tag><tag2>test</tag2></tag>";
            var value = _sut.HasChildren(stringToTest);
            var children = _sut.GetChildren(stringToTest);

            Assert.That(value, Is.True);
            Assert.That(children, Is.Not.Empty);
            Assert.That(children.Count, Is.EqualTo(1));
            Assert.That(children[0], Is.EqualTo("<tag2>test</tag2>"));
        }

        [Test]
        public void Children_1ChildForOpenedAndClosedVeryComplex()
        {
            var stringToTest = "<tag><tag2><tag3>test</tag3></tag2></tag>";
            var value = _sut.HasChildren(stringToTest);
            var children = _sut.GetChildren(stringToTest);

            Assert.That(value, Is.True);
            Assert.That(children, Is.Not.Empty);
            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children[0], Is.EqualTo("<tag2><tag3>test</tag3></tag2>"));
            Assert.That(children[1], Is.EqualTo("<tag3>test</tag3>"));
        }

        [Test]
        public void Children_1ChildForOpenedAndClosedVeryVeryComplex()
        {
            var stringToTest = "<tag><tag2><tag3>test</tag3><tag4>test2</tag4></tag2></tag>";
            var value = _sut.HasChildren(stringToTest);
            var children = _sut.GetChildren(stringToTest);

            Assert.That(value, Is.True);
            Assert.That(children, Is.Not.Empty);
            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children[0], Is.EqualTo("<tag2><tag3>test</tag3><tag4>test2</tag4></tag2>"));
            Assert.That(children[1], Is.EqualTo("<tag3>test</tag3>"));
        }

        [Test]
        public void Process_VeryComplexString()
        {
            var stringToTest = "<tag><tag2>test2</tag2><tag3>test3</tag3></tag>";
            var values = _sut.Process(stringToTest);

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(2));
            Assert.That(values["tag2"], Is.EqualTo("test2"));
            Assert.That(values["tag3"], Is.EqualTo("test3"));
        }

        [Test]
        public void Process_SimpleString()
        {
            var stringToTest = "<tag>test</tag>";
            var values = _sut.Process(stringToTest);

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values["tag"], Is.EqualTo("test"));
        }

        [Test]
        public void Process_SophisticatedString()
        {
            var stringToTest = "asdf<tag>test</tag>asdfx";
            var values = _sut.Process(stringToTest);

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Not.Empty);
            Assert.That(values.Values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values["tag"], Is.EqualTo("test"));
        }

        [Test]
        public void Process_NotThisTagString()
        {
            var stringToTest = "asdf<tag2>test</tag2>asdfx";
            var values = _sut.Process(stringToTest);

            Assert.That(values, Is.Not.Null);
            Assert.That(values.Keys, Is.Empty);
            Assert.That(values.Values, Is.Empty);
            Assert.That(values.Count, Is.EqualTo(0));
        }
    }
}