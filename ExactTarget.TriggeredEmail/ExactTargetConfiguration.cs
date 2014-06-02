namespace ExactTarget.TriggeredEmail
{
    public class ExactTargetConfiguration : IExactTargetConfiguration
    {
        public string EndPoint { get; set; }
        public int? ClientId { get; set; }
        public string ApiUserName { get; set; }
        public string ApiPassword { get; set; }
        public string SoapBinding { get; set; }
    }
}