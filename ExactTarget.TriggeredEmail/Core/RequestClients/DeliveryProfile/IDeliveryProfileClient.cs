namespace ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile
{
    public interface IDeliveryProfileClient
    {
        string CreateDeliveryProfile(string externalKey);
        bool DoesDeliveryProfileExist(string externalKey);
    }
}