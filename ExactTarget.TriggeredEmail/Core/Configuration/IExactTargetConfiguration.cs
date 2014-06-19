namespace ExactTarget.TriggeredEmail.Core.Configuration
{
    public interface IExactTargetConfiguration
    {
        string EndPoint { get; set; }
        int? ClientId { get; set; }
        string ApiUserName { get; set; }
        string ApiPassword { get; set; }
        string SoapBinding { get; set; }
    }
}