namespace ExactTarget.TriggeredEmail.Trigger
{
    public interface IEmailTrigger
    {
        void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueueing, Priority priority);
    }
}