using ExactTarget.TriggeredEmail.Creation;
using NUnit.Framework;

namespace ExactTarget.TriggeredEmail.Test.Unit.Creation
{
    [TestFixture]
    public class LayoutHtmlReplacementFieldNameParserTests
    {
        [TestCase("%%FirstName%%%%LastName%%%Field1%%%%Field2%%")]
        [TestCase("%%FirstName%% %%LastName%% %%Field1%% %%Field2%%")]
        [TestCase("%%FirstName%%,%%LastName%%,%%Field1%%,%%Field2%%")]
        [TestCase("%%FirstName%%, %%LastName%%, %%Field1%%, %%Field2%%")]
        [TestCase("%%FirstName%%, %%LastName%%, %%Field1%%, %%Field2%%, %% %% %%\n%%")]
        [TestCase("Hello %%FirstName%%, thank you %%LastName%% you have a 100% chance to win %%\n%%  <p>%%Field1%%</p><p> %%Field2%% </p>")]
        public void Parse_(string layoutHtml)
        {
            var result = LayoutHtmlReplacementFieldNameParser.Parse(layoutHtml);

            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result, Contains.Item("FirstName"));
            Assert.That(result, Contains.Item("LastName"));
            Assert.That(result, Contains.Item("Field1"));
            Assert.That(result, Contains.Item("Field2"));
        }
    }
}
