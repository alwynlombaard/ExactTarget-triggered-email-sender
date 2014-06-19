namespace ExactTarget.TriggeredEmail.Core.RequestClients.EmailTemplate
{
    public interface IEmailTemplateClient
    {
        int RetrieveEmailTemplateId(string externalKey);
        int CreateEmailTemplate(string externalKey, string name, string html);
    }
}