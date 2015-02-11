using System;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile
{
    public interface IDeliveryProfileClient : IDisposable
    {
        string TryCreateBlankDeliveryProfile(string externalKey);
    }
}