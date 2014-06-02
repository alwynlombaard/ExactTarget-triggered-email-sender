namespace ExactTarget.TriggeredEmail
{
    public interface IEmailTrigger
    {
        void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueueing);
    }
}