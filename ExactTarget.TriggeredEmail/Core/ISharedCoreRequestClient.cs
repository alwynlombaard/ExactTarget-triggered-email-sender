namespace ExactTarget.TriggeredEmail.Core
{
    public interface ISharedCoreRequestClient
    {
        bool DoesObjectExist(string propertyName, string value, string objectType);
    }
}