using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.Creation.Creators
{
    public interface IEmailCreator
    {
        int Create(string externalKey, string layoutHtml, Priority priority = Priority.Medium);
    }
}