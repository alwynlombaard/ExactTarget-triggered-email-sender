using System;
using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail.TestConsole
{
    public class Program
    {
        public static void Main()
        {
            //The triggered send external key (custtomer key) and recipient that the email will go to
            var triggeredEmail = new ExactTargetTriggeredEmail("external-key-of-trigger", "recipient@uri.test" );
            
            //if the email is attached to a data extension, add the values here (optional
            triggeredEmail.AddReplacementValues(new Dictionary<string, string>
                {
                    {"DataExtensionFieldName1","Value 1"}, 
                    {"DataExtensionFieldName2","Value 2"}
                });

            //get the config
            var config = GetConfig();

            //create trigger with config
            var emailTrigger = new EmailTrigger(config);

            
            //trigger the email
            try
            {
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
                ApiUserName = "API_User",
                ApiPassword = "API_Password",
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",//update your correct endpoint
                SoapBinding = "ExactTarget.Soap",
                ClientId = 100, // optional  business unit to use
            };
        }
    }
}
