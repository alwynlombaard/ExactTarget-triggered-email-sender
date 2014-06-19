namespace ExactTarget.TriggeredEmail
{
    public interface ITriggeredEmailCreator
    {
        int Create(string externalKey);
        void StartTriggeredSend(string externalKey);
    }
}