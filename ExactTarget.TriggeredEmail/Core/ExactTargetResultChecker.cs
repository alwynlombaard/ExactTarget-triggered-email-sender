using System;
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

            if (result != null
                && !result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception(string.Format("ExactTarget response indicates failure. StatusCode:{0} StatusMessage:{1}",
                    result.StatusCode,
                    result.StatusMessage));
            }
        }
    }
}