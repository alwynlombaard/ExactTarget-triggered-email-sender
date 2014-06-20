using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension
{
    public interface IDataExtensionClient
    {
        void CreateDataExtension(string dataExtensionTemplateObjectId,
            string externalKey,
            string name,
            HashSet<string> fields);

        bool DoesDataExtensionExist(string externalKey);
        
        string RetrieveTriggeredSendDataExtensionTemplateObjectId();
    }
}