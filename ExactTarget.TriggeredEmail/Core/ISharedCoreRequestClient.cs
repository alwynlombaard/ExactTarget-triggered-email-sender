namespace ExactTarget.TriggeredEmail.Core
{
    public interface ISharedCoreRequestClient
    {
        bool DoesObjectExist(string propertyName, string value, string objectType);
        string RetrieveObjectId(string propertyName, string value, string objectType);
    }
}