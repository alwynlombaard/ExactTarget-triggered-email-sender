namespace ExactTarget.TriggeredEmail.Core
{
    public interface ITriggeredSendDefinitionClient
    {
        int CreateTriggeredSendDefinition(string externalId,
            int emailId, 
            string dataExtensionCustomerKey,
            string name, 
            string description);

        bool DoesTriggeredSendDefinitionExist(string externalKey);

        string RetrieveTriggeredSendDataExtensionTemplateObjectId();
    }
}