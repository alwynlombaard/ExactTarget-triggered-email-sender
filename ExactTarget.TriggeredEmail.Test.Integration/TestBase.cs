using ExactTarget.TriggeredEmail.Core.Configuration;
using NUnit.Framework;

namespace ExactTarget.TriggeredEmail.Test.Integration
{
    [TestFixture]
    public class TestBase
    {
        public ExactTargetConfiguration Config {get; private set; }
        public string TestRecipientEmail { get; private set; }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            TestRecipientEmail = "";

            if (string.IsNullOrWhiteSpace(TestRecipientEmail))
            {
                Assert.Fail("You have to supply value for TestRecipientEmail before running these tests.");
            }

            Config = new ExactTargetConfiguration
            {
                ApiUserName = "",
                ApiPassword = "",
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",//update your correct endpoint
                ClientId = 6269489, // optional  business unit to use
            };

            if (string.IsNullOrWhiteSpace(Config.ApiUserName) || string.IsNullOrWhiteSpace(Config.ApiPassword))
            {
                Assert.Fail("You need to supply API credentials before running these tests.");
            }
            
        }
    }

    
}
