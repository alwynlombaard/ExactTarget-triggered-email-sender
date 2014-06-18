using System;
using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail.TestConsole
{
    public class Program
    {
        public static void Main()
        {
            const string externalKey = "my-test-external-key-2";
            try
            {
                var creator = new TriggeredEmailCreator(GetConfig());
                creator.Create(externalKey);
                creator.StartTriggeredSend(externalKey);

                var triggeredEmail = new ExactTargetTriggeredEmail(externalKey, "someone@temp.uri");
                triggeredEmail.AddReplacementValues(new Dictionary<string, string>
                {
                    {"Subject","Test email"}, 
                    {"Body","<h2>Test email heading</h2><p>Test paragraph</p>"}
                });
                
                var emailTrigger = new EmailTrigger(GetConfig());
                emailTrigger.Trigger(triggeredEmail);
                Console.WriteLine("Triggered external key {0} to {1} successfully", triggeredEmail.ExternalKey, triggeredEmail.EmailAddress);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            Console.ReadKey();
        }

        private static IExactTargetConfiguration GetConfig()
        {
            //load this from a config file
            return new ExactTargetConfiguration
            {
                ApiUserName = "",
                ApiPassword = "",
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",//update your correct endpoint
                ClientId = 6269490, // optional  business unit to use
            };
        }
    }
}
