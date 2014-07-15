using System;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.ExactTargetApi;
using NUnit.Framework;

namespace ExactTarget.TriggeredEmail.Test.Unit.Trigger
{
    [TestFixture]
    public class ExactTargetResultCheckerTests
    {
        [Test]
        public void CheckResultThrowsExceptionWhenResultIsNull()
        {
            var exception = Assert.Throws<Exception>(() => ExactTargetResultChecker.CheckResult(null));

            Assert.That(exception.Message, Is.EqualTo("Received an unexpected null result from ExactTarget"));
        }

        [Test]
        public void CheckResultDoesNotThrowExceptionIfResultStatusCodeIsOk()
        {
            var result = new Result{StatusCode = "OK"};
            Assert.DoesNotThrow(() => ExactTargetResultChecker.CheckResult(result));

            result = new Result { StatusCode = "ok" };
            Assert.DoesNotThrow(() => ExactTargetResultChecker.CheckResult(result));
        }

        [Test]
        public void CheckResultThrowsExceptionWhenResultStatusCodeIsNotOk()
        {
            var result = new Result {StatusCode = "Error", StatusMessage = "Reason for failing"};

            var ex = Assert.Throws<Exception>(() => ExactTargetResultChecker.CheckResult(result));
            Assert.That(ex.Message, Is.StringContaining("Error"));
            Assert.That(ex.Message, Is.StringContaining("Reason for failing"));
        }

        [Test]
        public void CheckResultThrowsExceptionWithSubscriberErrorIfPresent()
        {
            var result = new TriggeredSendCreateResult
            {
                StatusCode = "Error",
                StatusMessage = "Reason for failing",
                SubscriberFailures = new []{ new SubscriberResult{ErrorCode = "24", ErrorDescription = "Subscriber was excluded by List Detective"}}
            };

            var ex = Assert.Throws<Exception>(() => ExactTargetResultChecker.CheckResult(result));

            Assert.That(ex.Message, Is.StringContaining("Error"));
            Assert.That(ex.Message, Is.StringContaining("Reason for failing"));
            Assert.That(ex.Message, Is.StringContaining("24"));
            Assert.That(ex.Message, Is.StringContaining("Subscriber was excluded by List Detective"));

        }
    }
}
