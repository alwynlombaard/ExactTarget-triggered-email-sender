using System;
using System.Linq;
using ExactTarget.TriggeredEmail.Core.Exceptions;
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
            var subscriberFailures = triggeredResult == null || triggeredResult.SubscriberFailures == null
                ? new SubscriberResult[0]
                : triggeredResult.SubscriberFailures;

            var subscriberFailureMessages = subscriberFailures.Select(f => " ErrorCode:" + f.ErrorCode + " ErrorDescription:" + f.ErrorDescription);
            var exceptionMessage = string.Format("ExactTarget response indicates failure. StatusCode:{0} StatusMessage:{1} SubscriberFailures:{2}",
            result.StatusCode,
            result.StatusMessage,
            string.Join("|", subscriberFailureMessages));

            /* Error code 24 - List Detective Exclusion: The subscriber was excluded by List Detective.
             * This is a common error code returned by ExactTarget when the recipient is invalid.
             * See https://help.exacttarget.com/en/documentation/exacttarget/content/email_messages/email_send_error_codes/
             */
            const int listDetectiveExclusionErrorCode = 24;
            var lastFailure = subscriberFailures.LastOrDefault();
            if (lastFailure != null && 
                (lastFailure.ErrorCode == listDetectiveExclusionErrorCode.ToString() || lastFailure.ErrorDescription.StartsWith("Error Code: " + listDetectiveExclusionErrorCode)))
            {
                var subscriberEmailAddress = lastFailure.Subscriber == null ? null : lastFailure.Subscriber.EmailAddress;
                throw new SubscriberExcludedException(subscriberEmailAddress, exceptionMessage);
            }
            
            throw new ExactTargetException(exceptionMessage);
        }
    }
}