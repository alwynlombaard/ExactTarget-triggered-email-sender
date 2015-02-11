using System;
using System.Collections.Generic;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension;
using ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile;
using ExactTarget.TriggeredEmail.Core.RequestClients.Email;
using ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition;
using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.Creation
{
    public class PasteHtmlTriggeredEmailCreator : ITriggeredEmailCreator, IDisposable
    {
        private readonly ITriggeredSendDefinitionClient _triggeredSendDefinitionClient;
        private readonly IDataExtensionClient _dataExtensionClient;
        private readonly IEmailRequestClient _emailRequestClient;
        private readonly IDeliveryProfileClient _deliveryProfileClient;

        public PasteHtmlTriggeredEmailCreator(IDataExtensionClient dataExtensionClient,
            ITriggeredSendDefinitionClient triggeredSendDefinitionClient,
            IEmailRequestClient emailRequestClient,
            IDeliveryProfileClient deliveryProfileClient)
        {
            _dataExtensionClient = dataExtensionClient;
            _triggeredSendDefinitionClient = triggeredSendDefinitionClient;
            _emailRequestClient = emailRequestClient;
            _deliveryProfileClient = deliveryProfileClient;
        }

        public PasteHtmlTriggeredEmailCreator(IExactTargetConfiguration config)
            : this( new DataExtensionClient(config),
                    new TriggeredSendDefinitionClient(config),
                    new EmailRequestClient(config),
                    new DeliveryProfileClient(config)
            )
        {
        }

        public int Create(string externalKey, string layoutHtml, Priority priority = Priority.Medium)
        {
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException(
                    "externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (_triggeredSendDefinitionClient.DoesTriggeredSendDefinitionExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits",
                    externalKey));
            }

            var dataExtensionExternalKey = ExternalKeyGenerator.GenerateExternalKey("data-extension-" + externalKey);
            if (!_dataExtensionClient.DoesDataExtensionExist(dataExtensionExternalKey))
            {
                var dataExtensionTemplateObjectId =_dataExtensionClient.RetrieveTriggeredSendDataExtensionTemplateObjectId();
                var dataExtensionFieldNames = new HashSet<string> {"Subject"};

                var replacementFieldNames = LayoutHtmlReplacementFieldNameParser.Parse(layoutHtml);
                foreach (var replacementFieldName in replacementFieldNames)
                {
                    dataExtensionFieldNames.Add(replacementFieldName);
                }

                _dataExtensionClient.CreateDataExtension(dataExtensionTemplateObjectId,
                    dataExtensionExternalKey,
                    "triggeredsend-" + externalKey,
                    dataExtensionFieldNames);
            }

            var emailName = "email-" + externalKey;
            var emailExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-" + externalKey);

            layoutHtml +=   EmailContentHelper.GetOpenTrackingTag() +
                            EmailContentHelper.GetCompanyPhysicalMailingAddressTags();

            var emailId = _emailRequestClient.CreateEmail(emailExternalKey, emailName, "%%Subject%%",
               layoutHtml);

            var deliveryProfileExternalKey = ExternalKeyGenerator.GenerateExternalKey("blank-delivery-profile");
            _deliveryProfileClient.TryCreateBlankDeliveryProfile(deliveryProfileExternalKey);

            return _triggeredSendDefinitionClient.CreateTriggeredSendDefinition(externalKey,
                emailId,
                dataExtensionExternalKey,
                deliveryProfileExternalKey,
                externalKey,
                externalKey,
                priority.ToString());
        }

        public void Dispose()
        {
            _dataExtensionClient.Dispose();
            _deliveryProfileClient.Dispose();
            _emailRequestClient.Dispose();
            _triggeredSendDefinitionClient.Dispose();
        }
    }
}
