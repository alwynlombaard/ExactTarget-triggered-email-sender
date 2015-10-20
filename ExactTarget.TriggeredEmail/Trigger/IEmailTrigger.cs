using System.Threading.Tasks;

namespace ExactTarget.TriggeredEmail.Trigger
{
    public interface IEmailTrigger
    {
        void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueuing, Priority priority);

        Task TriggerAsync(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueuing, Priority priority);
    }
}