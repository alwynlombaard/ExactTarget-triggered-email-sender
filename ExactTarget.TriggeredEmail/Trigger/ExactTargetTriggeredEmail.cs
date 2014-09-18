using System;
using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail.Trigger
{
    public class ExactTargetTriggeredEmail
    {
        public ExactTargetTriggeredEmail(string externalKey, string emailAddress)
        {
            if (string.IsNullOrEmpty(externalKey))
            {
                throw new ArgumentException("externalkey is required", "externalKey");
            }
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentException("emailAddress is required", "emailAddress");
            }
            ExternalKey = externalKey;
            EmailAddress = emailAddress;
            ReplacementValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Specifies the sender email address. 
        /// Both FromAddress and FromName must be specified to set the email sender field.
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Specifies the sender name.
        /// Both FromAddress and FromName must be specified to set the email sender field.
        /// </summary>
        public string FromName { get; set; }

        public string EmailAddress { get; private set; }

        public string SubscriberKey { get; set; }

        public string ExternalKey { get; private set; }

        public Dictionary<string, string> ReplacementValues { get; private set; }

        public ExactTargetTriggeredEmail AddReplacementValue(string key, string value)
        {
            ReplacementValues[key] = value;
            return this;
        }

        public ExactTargetTriggeredEmail AddReplacementValues(IEnumerable<KeyValuePair<string, string>> tags)
        {
            foreach (var pair in tags)
            {
                AddReplacementValue(pair.Key, pair.Value);
            }
            return this;
        }
    }
}
