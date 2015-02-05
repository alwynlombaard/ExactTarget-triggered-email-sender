using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension;
using ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile;
using ExactTarget.TriggeredEmail.Core.RequestClients.Email;
using ExactTarget.TriggeredEmail.Core.RequestClients.EmailTemplate;
using ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition;

namespace ExactTarget.TriggeredEmail.Creation {
	public class DynamicTriggeredEmailCreator {
		private readonly DataExtensionClient dataExtensionClient;
		private readonly DeliveryProfileClient deliveryProfileClient;
		private readonly EmailRequestClient emailRequestClient;
		private readonly EmailTemplateClient emailTemplateClient;
		private readonly TriggeredSendDefinitionClient triggeredSendDefinitionClient;

		public DynamicTriggeredEmailCreator(IExactTargetConfiguration config) {
			triggeredSendDefinitionClient = new TriggeredSendDefinitionClient(config);
			dataExtensionClient = new DataExtensionClient(config);
			emailTemplateClient = new EmailTemplateClient(config);
			emailRequestClient = new EmailRequestClient(config);
			deliveryProfileClient = new DeliveryProfileClient(config);
		}

		public int Create(string externalKey, string layoutHtml) {
			if (externalKey.Length > Guid.Empty.ToString().Length) {
				throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
			}

			if (triggeredSendDefinitionClient.DoesTriggeredSendDefinitionExist(externalKey)) {
				throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
			}

			var dataExtensionExternalKey = ExternalKeyGenerator.GenerateExternalKey("data-extension-" + externalKey);
			if (!dataExtensionClient.DoesDataExtensionExist(dataExtensionExternalKey)) {
				var dataExtensionTemplateObjectId = dataExtensionClient.RetrieveTriggeredSendDataExtensionTemplateObjectId();

				var regex = new Regex(@"(?<=%%)[^\s].*?[^\s]?(?=%%)");
				var matches = regex.Matches(layoutHtml);
				var dataExtensionFieldNames = new HashSet<string> { "Subject", "Body", "Head" };

				for (var i = 0; i < matches.Count; i++) {
					dataExtensionFieldNames.Add(matches[i].Value);
				}

				dataExtensionClient.CreateDataExtension(dataExtensionTemplateObjectId,
					dataExtensionExternalKey,
					"triggeredsend-" + externalKey,
					dataExtensionFieldNames);
			}


			var emailTempalteExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-template" + externalKey);
			var emailTemplateId = emailTemplateClient.RetrieveEmailTemplateId(emailTempalteExternalKey);
			if (emailTemplateId == 0) {
				layoutHtml += EmailContentHelper.GetOpenTrackingTag() + EmailContentHelper.GetCompanyPhysicalMailingAddressTags();
				emailTemplateId = emailTemplateClient.CreateEmailTemplate(emailTempalteExternalKey, "template-" + externalKey, layoutHtml);
			}

			var emailId = emailRequestClient.CreateEmailFromTemplate(emailTemplateId, "email-" + externalKey, "%%Subject%%", new KeyValuePair<string, string>("dynamicArea", "%%Body%%"));

			var deliveryProfileExternalKey = ExternalKeyGenerator.GenerateExternalKey("blank-delivery-profile");
			deliveryProfileClient.TryCreateBlankDeliveryProfile(deliveryProfileExternalKey);
			var triggeredSendDefinition = triggeredSendDefinitionClient.CreateTriggeredSendDefinition(externalKey, emailId, dataExtensionExternalKey, deliveryProfileExternalKey, externalKey, externalKey, "High");
			triggeredSendDefinitionClient.StartTriggeredSend(externalKey);
			return triggeredSendDefinition;
		}
	}
}