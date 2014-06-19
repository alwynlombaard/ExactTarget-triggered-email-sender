using System.Collections.Generic;

namespace ExactTarget.TriggeredEmail.Core
{
    public interface IEmailRequestClient
    {
        int CreateEmailFromTemplate(int emailTemplateId, string emailName, string subject, KeyValuePair<string, string> contentArea);
    }
}