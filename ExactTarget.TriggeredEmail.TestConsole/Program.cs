using System;
using System.Collections.Generic;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.Creation;
using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.TestConsole
{
    public class Program
    {
        public static void Main()
        {
            const string externalKey = "my-test-external-key-5";
            try
            {
                CreateTriggeredSend(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            try
            {
                StartTriggeredSend(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            try
            {
                Send(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.ReadKey();
        }

        private static void CreateTriggeredSend(string externalKey)
        {
            var triggeredEmailCreator = new TriggeredEmailCreator(GetConfig());
            triggeredEmailCreator.Create(externalKey);
            Console.WriteLine("Completed creating triggered send");
        }

        private static void StartTriggeredSend(string externalKey)
        {
            var triggeredEmailCreator = new TriggeredEmailCreator(GetConfig());
            triggeredEmailCreator.StartTriggeredSend(externalKey);
            Console.WriteLine("Started triggered send");
        }

        private static void Send(string externalKey)
        {
            var triggeredEmail = new ExactTargetTriggeredEmail(externalKey, "recipient@test.uri");
            triggeredEmail.AddReplacementValues(new Dictionary<string, string>
                {
                    {"Subject","Test email"}, 
                    {"Body","<h2>Test email heading</h2><p>Test paragraph</p>"}
                });

            var emailTrigger = new EmailTrigger(GetConfig());
            emailTrigger.Trigger(triggeredEmail);
            Console.WriteLine("Triggered external key {0} to {1} successfully", triggeredEmail.ExternalKey, triggeredEmail.EmailAddress);
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
