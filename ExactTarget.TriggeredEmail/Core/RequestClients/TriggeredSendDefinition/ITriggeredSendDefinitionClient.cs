namespace ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition
{
    public interface ITriggeredSendDefinitionClient
    {
        int CreateTriggeredSendDefinition(string externalId,
            int emailId, 
            string dataExtensionCustomerKey,
            string deliveryProfileCustomerKey,
            string name, 
            string description);

        bool DoesTriggeredSendDefinitionExist(string externalKey);

        void StartTriggeredSend(string externalKey);
    }
}