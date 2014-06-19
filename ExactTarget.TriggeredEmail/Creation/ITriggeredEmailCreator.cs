namespace ExactTarget.TriggeredEmail.Creation
{
    public interface ITriggeredEmailCreator
    {
        int Create(string externalKey);
        void StartTriggeredSend(string externalKey);
    }
}