namespace ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile
{
    public interface IDeliveryProfileClient
    {
        string TryCreateBlankDeliveryProfile(string externalKey);
    }
}