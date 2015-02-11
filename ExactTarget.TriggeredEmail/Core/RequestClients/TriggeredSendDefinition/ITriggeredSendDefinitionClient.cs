using System;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition
{
    public interface ITriggeredSendDefinitionClient : IDisposable
    {
        int CreateTriggeredSendDefinition(string externalId,
            int emailId, 
            string dataExtensionCustomerKey,
            string deliveryProfileCustomerKey,
            string name, 
            string description,
			string priority="");

        bool DoesTriggeredSendDefinitionExist(string externalKey);

        void StartTriggeredSend(string externalKey);
    }
}