using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;
using Attribute = ExactTarget.TriggeredEmail.ExactTargetApi.Attribute;

namespace ExactTarget.TriggeredEmail
{
    public enum RequestQueueing
    {
        No = 0,
        Yes,
    }

    public class EmailTrigger : IEmailTrigger
    {
        private readonly IExactTargetConfiguration _config;

        public EmailTrigger(IExactTargetConfiguration config)
        {
            _config = config;
        }

        public void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueueing = RequestQueueing.No)
        {
            var clientId = _config.ClientId;
            var client = new SoapClient(_config.SoapBinding, _config.EndPoint);
            if (client.ClientCredentials != null)
            {
                client.ClientCredentials.UserName.UserName = _config.ApiUserName;
                client.ClientCredentials.UserName.Password = _config.ApiPassword;
            }

            var subscribers = new List<Subscriber>
                {
                    new Subscriber
                        {
                            EmailAddress = exactTargetTriggeredEmail.EmailAddress,
                            SubscriberKey = exactTargetTriggeredEmail.SubscriberKey ?? exactTargetTriggeredEmail.EmailAddress,
                            Attributes =
                                exactTargetTriggeredEmail.ReplacementValues.Select(value => new Attribute
                                    {
                                        Name = value.Key,
                                        Value = value.Value
                                    }).ToArray()
                        }
                };

            var tsd = new TriggeredSendDefinition
            {
                Client = clientId.HasValue ?  new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                CustomerKey = exactTargetTriggeredEmail.ExternalKey
            };

            var ts = new TriggeredSend
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                TriggeredSendDefinition = tsd,
                Subscribers = subscribers.ToArray()
            };

            var co = new CreateOptions
            {
                RequestType = requestQueueing == RequestQueueing.No ? RequestType.Synchronous : RequestType.Asynchronous,
                RequestTypeSpecified = true,
                QueuePriority = Priority.High,
                QueuePrioritySpecified = true
            };

            string requestId, status;
            var result = client.Create(
                co,
                new APIObject[] { ts },
                out requestId, out status);

            CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }

        private void CheckResult(Result result)
        {
            if (result == null)
            {
                throw new Exception("Recieved an unexpected null result from ExactTarget");
            }

            if (result != null 
                && !result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                    throw new Exception(string.Format("ExactTarget response indicates failure. StatusCode:{0} StatusMessage:{1}",
                                            result.StatusCode,
                                            result.StatusMessage));
            }
        }
    }
}
