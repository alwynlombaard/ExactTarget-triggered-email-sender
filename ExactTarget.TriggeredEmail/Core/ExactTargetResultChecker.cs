using System;
using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core
{
    public class ExactTargetResultChecker
    {
        public static void CheckResult(Result result)
        {
            if (result == null)
            {
                throw new Exception("Received an unexpected null result from ExactTarget");
            }

            if (result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            var triggeredResult = result as TriggeredSendCreateResult;
            var subscriberFailures = triggeredResult == null
                ? Enumerable.Empty<string>()
                : triggeredResult.SubscriberFailures.Select(f => " ErrorCode:" + f.ErrorCode + " ErrorDescription:" + f.ErrorDescription);

            throw new Exception(string.Format("ExactTarget response indicates failure. StatusCode:{0} StatusMessage:{1} SubscriberFailures:{2}",
                result.StatusCode,
                result.StatusMessage,
                string.Join("|", subscriberFailures)));
        }
    }
}