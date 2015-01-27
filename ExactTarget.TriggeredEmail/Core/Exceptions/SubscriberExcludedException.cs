namespace ExactTarget.TriggeredEmail.Core.Exceptions
{
    public class SubscriberExcludedException : ExactTargetException
    {
        private readonly string _emailAddress;
        private readonly string _additionalInfo;

        public SubscriberExcludedException(string emailAddress)
        {
            _emailAddress = emailAddress;
        }

        public SubscriberExcludedException(string emailAddress, string additionalInfo)
        {
            _emailAddress = emailAddress;
            _additionalInfo = additionalInfo;
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
        }

        public override string Message
        {
            get { return string.Format("Subcriber was excluded. EmailAddress: {0} Additional Info: {1}", _emailAddress, _additionalInfo); }
        }
    }
}
