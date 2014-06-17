using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail
{
    public interface ITriggeredEmailCreator
    {
        void Create(string externalKey, IEnumerable<string> dataExtensionFieldNames);
    }
}