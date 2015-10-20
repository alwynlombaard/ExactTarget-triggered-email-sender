using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.ExactTargetApi;
using Attribute = ExactTarget.TriggeredEmail.ExactTargetApi.Attribute;

namespace ExactTarget.TriggeredEmail.Trigger
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

        public void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueuing = RequestQueueing.No, Priority priority = Priority.Medium)
        {
            var ts = GetTriggeredSendObject(exactTargetTriggeredEmail, priority);
            var co = GetCreateOptions(requestQueuing, priority);

            using (var client = SoapClientFactory.Manufacture(_config))
            {
                string requestId;
                string status;

                var result = client.Create(
                    co,
                    new APIObject[] {ts},
                    out requestId, out status);

                ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
            }
        }

        public async Task TriggerAsync(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueuing = RequestQueueing.No, Priority priority = Priority.Medium)
        {
            var ts = GetTriggeredSendObject(exactTargetTriggeredEmail, priority);
            var co = GetCreateOptions(requestQueuing, priority);

            using (var client = SoapClientFactory.Manufacture(_config))
            {
                var response = await client.CreateAsync(new CreateRequest(co, new APIObject[] {ts}));

                ExactTargetResultChecker.CheckResult(response.Results.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
            }
        }

        private TriggeredSend GetTriggeredSendObject(ExactTargetTriggeredEmail exactTargetTriggeredEmail, Priority priority)
        {
            var clientId = _config.ClientId;

            var subscriber = new Subscriber
            {
                EmailAddress = exactTargetTriggeredEmail.EmailAddress,
                SubscriberKey = exactTargetTriggeredEmail.SubscriberKey ?? exactTargetTriggeredEmail.EmailAddress,
                Attributes =
                    exactTargetTriggeredEmail.ReplacementValues.Select(value => new Attribute
                    {
                        Name = value.Key,
                        Value = value.Value
                    }).ToArray()
            };

            // Add sender information if specified. This will send the email with FromAddress in the sender field.
            // Official doco here under the section "Determining the From Information at Send Time":
            // https://help.exacttarget.com/en/technical_library/web_service_guide/triggered_email_scenario_guide_for_developers/#Determining_the_From_Information_at_Send_Time
            if (!string.IsNullOrEmpty(exactTargetTriggeredEmail.FromAddress) &&
                !string.IsNullOrEmpty(exactTargetTriggeredEmail.FromName))
            {
                subscriber.Owner = new Owner
                {
                    FromAddress = exactTargetTriggeredEmail.FromAddress,
                    FromName = exactTargetTriggeredEmail.FromName
                };
            }

            var subscribers = new List<Subscriber> {subscriber};

            var tsd = new TriggeredSendDefinition
            {
                Client = clientId.HasValue ? new ClientID {ID = clientId.Value, IDSpecified = true} : null,
                CustomerKey = exactTargetTriggeredEmail.ExternalKey,
                Priority = priority == Priority.High ? "High" : "Medium",
            };

            var ts = new TriggeredSend
            {
                Client = clientId.HasValue ? new ClientID {ID = clientId.Value, IDSpecified = true} : null,
                TriggeredSendDefinition = tsd,
                Subscribers = subscribers.ToArray()
            };
            return ts;
        }

        private static CreateOptions GetCreateOptions(RequestQueueing requestQueuing, Priority priority)
        {
            var co = new CreateOptions
            {
                RequestType = requestQueuing == RequestQueueing.No ? RequestType.Synchronous : RequestType.Asynchronous,
                RequestTypeSpecified = true,
                QueuePriority = priority == Priority.High ? ExactTargetApi.Priority.High : ExactTargetApi.Priority.Medium,
                QueuePrioritySpecified = true,
                PrioritySpecified = true,
                Priority = (sbyte) (priority == Priority.High ? 2 : 1),
            };
            return co;
        }
    }
}
