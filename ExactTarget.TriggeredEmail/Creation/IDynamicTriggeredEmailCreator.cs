using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.Creation
{
    public interface IDynamicTriggeredEmailCreator
    {
        int Create(string externalKey, string layoutHtml, Priority priority = Priority.Medium);
    }
}