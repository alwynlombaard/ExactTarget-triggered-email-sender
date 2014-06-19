using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension
{
    public interface IDataExtensionClient
    {
        void CreateDataExtension(int? clientId,
            string dataExtensionTemplateObjectId,
            string externalKey,
            string name,
            IEnumerable<string> fields);

        bool DoesDataExtensionExist(string externalKey);
        
        string RetrieveTriggeredSendDataExtensionTemplateObjectId();
    }
}