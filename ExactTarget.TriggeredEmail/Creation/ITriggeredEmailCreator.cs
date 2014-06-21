namespace ExactTarget.TriggeredEmail.Creation
{
    public interface ITriggeredEmailCreator
    {
        int Create(string externalKey);
        int CreateTriggeredSendDefinitionWithEmailTemplate(string externalKey, string layoutHtmlAboveBodyTag, string layoutHtmlBelowBodyTag);
        int CreateTriggeredSendDefinitionWithPasteHtml(string externalKey);
        void StartTriggeredSend(string externalKey);
    }
}