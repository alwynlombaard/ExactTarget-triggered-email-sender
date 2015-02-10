using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.Creation
{
    public interface ITriggeredEmailCreator
    {
        int Create(string externalKey, string layoutHtml, Priority priority = Priority.Medium);
    }
}