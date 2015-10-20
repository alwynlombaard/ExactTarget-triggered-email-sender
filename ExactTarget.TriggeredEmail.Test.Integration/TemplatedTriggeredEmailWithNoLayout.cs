using System;
using System.Threading.Tasks;
using ExactTarget.TriggeredEmail.Creation;
using ExactTarget.TriggeredEmail.Trigger;
using NUnit.Framework;

namespace ExactTarget.TriggeredEmail.Test.Integration
{
    public class TemplatedTriggeredEmailWithNoLayout : TestBase
    {
        [Test]
        public void Create_And_Send_A_Templated_Triggered_Email_With_No_Layout()
        {
            var externalKey = Guid.NewGuid().ToString();
            Create(externalKey);
            Send(externalKey);
        }

        [Test]
        public void Create_And_Send_A_Templated_Triggered_Email_With_No_Layout_Async()
        {
            var externalKey = Guid.NewGuid().ToString();
            Create(externalKey);
            SendAsync(externalKey);
        }

        private void Create(string externalKey)
        {
            var triggeredEmailCreator = new TriggeredEmailCreator(Config);

            Assert.DoesNotThrow(() => triggeredEmailCreator.CreateTriggeredSendDefinitionWithEmailTemplate(
                                        externalKey,                        
                                        "<html><head></head><style>.green{color:green}</style>",
                                        "</html>",
                                        Priority.High),
                                    "Failed to create Triggered Email");

            Assert.DoesNotThrow(() => triggeredEmailCreator.StartTriggeredSend(externalKey), "Failed to start Triggered Send");
        }

        private void Send(string externalKey)
        {
            var triggeredEmail = new ExactTargetTriggeredEmail(externalKey, TestRecipientEmail);
            triggeredEmail.AddReplacementValue("Subject", "Test email");
            triggeredEmail.AddReplacementValue("Body", "<p class='green'>Some test copy in green</p>" +
                                                       "<p>This is a Templated email without layout.</p>");

            var emailTrigger = new EmailTrigger(Config);
            
            Assert.DoesNotThrow( () =>  emailTrigger.Trigger(triggeredEmail), "Failed to send email");
        }

        private void SendAsync(string externalKey)
        {
            var triggeredEmail = new ExactTargetTriggeredEmail(externalKey, TestRecipientEmail);
            triggeredEmail.AddReplacementValue("Subject", "Test email");
            triggeredEmail.AddReplacementValue("Body", "<p class='green'>Some test copy in green</p>" +
                                                       "<p>This is a Templated email without layout.</p>");

            var emailTrigger = new EmailTrigger(Config);

            Assert.DoesNotThrow(() => Task.WaitAll(emailTrigger.TriggerAsync(triggeredEmail)), "Failed to send email");
        }
    }
}
