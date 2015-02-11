using System;
using System.Linq;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile
{
    public class DeliveryProfileClient : IDeliveryProfileClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public DeliveryProfileClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
        }

        public string TryCreateBlankDeliveryProfile(string externalKey)
        {
            try
            {
                var dp = new ExactTargetApi.DeliveryProfile
                {
                    Client =
                        _config.ClientId.HasValue
                            ? new ClientID {ID = _config.ClientId.Value, IDSpecified = true}
                            : null,
                    Name = "Blank delivery profile",
                    Description = "Blank delivery profile",
                    CustomerKey = externalKey,
                    FooterSalutationSource = SalutationSourceEnum.None,
                    FooterSalutationSourceSpecified = true,
                    HeaderSalutationSource = SalutationSourceEnum.None,
                    HeaderSalutationSourceSpecified = true,
                    SourceAddressType = DeliveryProfileSourceAddressTypeEnum.DefaultPrivateIPAddress,
                    SourceAddressTypeSpecified = true
                };

                string requestId, status;
                var result = _client.Create(new CreateOptions(), new APIObject[] {dp}, out requestId, out status);

                ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
                //we expect only one result because we've sent only one APIObject
                return result.First().NewObjectID;
            }
            catch
            {
                return Guid.Empty.ToString();
            }
        }

        public void Dispose()
        {
            using (_client) { }
        }
    }
}
