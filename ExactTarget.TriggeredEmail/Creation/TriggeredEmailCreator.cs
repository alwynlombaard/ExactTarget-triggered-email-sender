using System;
using System.Collections.Generic;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension;
using ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile;
using ExactTarget.TriggeredEmail.Core.RequestClients.Email;
using ExactTarget.TriggeredEmail.Core.RequestClients.EmailTemplate;
using ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition;
using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.Creation
{
    public class TriggeredEmailCreator : ITriggeredEmailCreator
    {
        private readonly ITriggeredSendDefinitionClient _triggeredSendDefinitionClient;
        private readonly IDataExtensionClient _dataExtensionClient;
        private readonly IEmailTemplateClient _emailTemplateClient;
        private readonly IEmailRequestClient _emailRequestClient;
        private readonly IDeliveryProfileClient _deliveryProfileClient;
        private readonly IDynamicTriggeredEmailCreator _dynamicTriggeredEmailCreator;

        public TriggeredEmailCreator(IDataExtensionClient dataExtensionClient,
            ITriggeredSendDefinitionClient triggeredSendDefinitionClient,
            IEmailTemplateClient emailTemplateClient,
            IEmailRequestClient emailRequestClient,
            IDeliveryProfileClient deliveryProfileClient,
            IDynamicTriggeredEmailCreator dynamicTriggeredEmailCreator)
        {
            _dataExtensionClient = dataExtensionClient;
            _triggeredSendDefinitionClient = triggeredSendDefinitionClient;
            _emailTemplateClient = emailTemplateClient;
            _emailRequestClient = emailRequestClient;
            _deliveryProfileClient = deliveryProfileClient;
            _dynamicTriggeredEmailCreator = dynamicTriggeredEmailCreator;
        }

        public TriggeredEmailCreator(IExactTargetConfiguration config)
            : this(new DataExtensionClient(config),
                new TriggeredSendDefinitionClient(config),
                new EmailTemplateClient(config),
                new EmailRequestClient(config),
                new DeliveryProfileClient(config),
                new DynamicTriggeredEmailCreator(config))
        {
           
        }

        /// <summary>
        /// <para>Creates a Triggered Send Definition with an email template containing a content area.</para>
        /// <para>Use this if you want to edit the email markup in the ET UI.  </para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public int Create(string externalKey, Priority priority = Priority.Medium)
        {
            return CreateWithTemplate(externalKey, 
                "<html>" +
                "<body>" +
                EmailContentHelper.GetContentAreaTag("dynamicArea") +
                EmailContentHelper.GetOpenTrackingTag() +
                EmailContentHelper.GetCompanyPhysicalMailingAddressTags() +
                "</body>" +
                "</html>",
                priority);
        }

        /// <summary>
        /// <para>Creates a Triggered Send Definition with an email template containing the supplied mark up.</para>
        /// <para>Use this if you do not wish to log in to ET UI to edit the email markup.</para>
        /// <para>Replacement values in layoutHtml (for example %%FirstName%% or %%myOwnVariableName%% ) 
        /// will be parsed and created as fields in the Data Extension.</para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="layoutHtml">Replacement values in layoutHtml (for example %%FirstName%% or %%MyOwnVariableName%% ) 
        /// will be parsed and corresponding fields created in the Data Extension with that name.</param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public int Create(string externalKey, string layoutHtml, Priority priority = Priority.Medium)
        {
            return _dynamicTriggeredEmailCreator.Create(externalKey, layoutHtml, priority);
        }

        public int CreateTriggeredSendDefinitionWithEmailTemplate(string externalKey, string layoutHtmlAboveBodyTag, string layoutHtmlBelowBodyTag, Priority priority = Priority.Medium)
        {
            return CreateWithTemplate(externalKey,
                layoutHtmlAboveBodyTag +
                "<body>" +
                EmailContentHelper.GetContentAreaTag("dynamicArea") +
                EmailContentHelper.GetOpenTrackingTag() +
                EmailContentHelper.GetCompanyPhysicalMailingAddressTags() + 
                "</body>" +
                layoutHtmlBelowBodyTag,
                priority);
        }

        

        public int CreateTriggeredSendDefinitionWithPasteHtml(string externalKey, Priority priority = Priority.Medium)
        {
            return CreateWithoutTemplate(externalKey, priority);
        }

        public void StartTriggeredSend(string externalKey)
        {
            _triggeredSendDefinitionClient.StartTriggeredSend(externalKey);
        }

        private int CreateWithTemplate(string externalKey, string layoutHtml, Priority priority)
        {
            return CreateTheEmail(externalKey, layoutHtml, priority);
        }

        private int CreateWithoutTemplate(string externalKey, Priority priority)
        {
            return CreateTheEmail(externalKey, null, priority);
        }

        private int CreateTheEmail(string externalKey, string layoutHtml, Priority priority)
        {
            var isTemplated = !string.IsNullOrWhiteSpace(layoutHtml);
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (_triggeredSendDefinitionClient.DoesTriggeredSendDefinitionExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
            }

            var dataExtensionExternalKey = ExternalKeyGenerator.GenerateExternalKey("data-extension-" + externalKey);
            if (!_dataExtensionClient.DoesDataExtensionExist(dataExtensionExternalKey))
            {
                var dataExtensionTemplateObjectId = _dataExtensionClient.RetrieveTriggeredSendDataExtensionTemplateObjectId();
                var dataExtensionFieldNames = new HashSet<string> { "Subject", "Body" };
                if (!isTemplated)
                {
                    dataExtensionFieldNames.Add("Head");
                }

                _dataExtensionClient.CreateDataExtension(dataExtensionTemplateObjectId,
                                    dataExtensionExternalKey,
                                    "triggeredsend-" + externalKey,
                                    dataExtensionFieldNames);

            }


            int emailId;
            var emailName = "email-" + externalKey;
            var emailExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-" + externalKey);
            if (isTemplated)
            {
                var emailTempalteExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-template" + externalKey);
                var emailTemplateId = _emailTemplateClient.RetrieveEmailTemplateId(emailTempalteExternalKey);
                if (emailTemplateId == 0)
                {
                    emailTemplateId = _emailTemplateClient.CreateEmailTemplate(emailTempalteExternalKey,
                        "template-" + externalKey,
                        layoutHtml);
                }

                emailId = _emailRequestClient.CreateEmailFromTemplate(emailTemplateId,
                    emailName,
                    "%%Subject%%",
                    new KeyValuePair<string, string>("dynamicArea", "%%Body%%"));
            }
            else
            {
                emailId = _emailRequestClient.CreateEmail(emailExternalKey, emailName, "%%Subject%%",
                    "<html>" +
                    "<head>%%Head%%</head>" +
                    "%%Body%%" +
                    EmailContentHelper.GetOpenTrackingTag() +
                    EmailContentHelper.GetCompanyPhysicalMailingAddressTags() +
                    "</html>");
            }

            
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
    }
}