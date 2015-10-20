using System;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.Core.Exceptions;
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
            var result = new Result {StatusCode = "OK"};
            Assert.DoesNotThrow(() => ExactTargetResultChecker.CheckResult(result));

            result = new Result {StatusCode = "ok"};
            Assert.DoesNotThrow(() => ExactTargetResultChecker.CheckResult(result));
        }

        [Test]
        public void CheckResultThrowsExceptionWhenResultStatusCodeIsNotOk()
        {
            var result = new Result {StatusCode = "Error", StatusMessage = "Reason for failing"};

            var ex = Assert.Throws<ExactTargetException>(() => ExactTargetResultChecker.CheckResult(result));
            Assert.That(ex.Message, Is.StringContaining("Error"));
            Assert.That(ex.Message, Is.StringContaining("Reason for failing"));
        }

        [Test]
        public void CheckResultThrowsExceptionWithGenericSubscriberError()
        {
            var result = new TriggeredSendCreateResult
            {
                StatusCode = "Error",
                StatusMessage = "Reason for failing",
                SubscriberFailures = new[]
                {
                    new SubscriberResult {ErrorCode = "00", ErrorDescription = "Generic subscriber failure"}
                }
            };

            var ex = Assert.Throws<ExactTargetException>(() => ExactTargetResultChecker.CheckResult(result));

            Assert.That(ex.Message, Is.StringContaining("Error"));
            Assert.That(ex.Message, Is.StringContaining("Reason for failing"));
            Assert.That(ex.Message, Is.StringContaining("00"));
            Assert.That(ex.Message, Is.StringContaining("Generic subscriber failure"));
        }

        [Test]
        public void CheckResultThrowsExceptionWithSubscriberExcludedError()
        {
            var result = new TriggeredSendCreateResult
            {
                StatusCode = "Error",
                StatusMessage = "Reason for failing",
                SubscriberFailures = new[]
                {
                    new SubscriberResult
                    {
                        ErrorCode = "24",
                        ErrorDescription = "Subscriber was excluded by List Detective",
                        Subscriber = new Subscriber {EmailAddress = "email@address.com"}
                    }
                }
            };

            var ex = Assert.Throws<SubscriberExcludedException>(() => ExactTargetResultChecker.CheckResult(result));

            Assert.AreEqual("email@address.com", ex.EmailAddress);
            Assert.AreEqual(
                "Subcriber was excluded. EmailAddress: email@address.com Additional Info: ExactTarget response indicates failure. StatusCode:Error StatusMessage:Reason for failing SubscriberFailures: ErrorCode:24 ErrorDescription:Subscriber was excluded by List Detective",
                ex.Message);
        }

        [Test]
        public void CheckResultThrowsExceptionWithSubscriberExcludedErrorEvenIfThereAreMultipleSubscriberResults()
        {
            var result = new TriggeredSendCreateResult
            {
                StatusCode = "Error",
                StatusMessage = "Reason for failing",
                SubscriberFailures = new[]
                {
                    new SubscriberResult
                    {
                        ErrorCode = "123",
                        ErrorDescription = "Some other failure"
                    },
                    new SubscriberResult
                    {
                        ErrorCode = "24",
                        ErrorDescription = "Subscriber was excluded by List Detective",
                        Subscriber = new Subscriber {EmailAddress = "email@address.com"}
                    }
                }
            };

            var ex = Assert.Throws<SubscriberExcludedException>(() => ExactTargetResultChecker.CheckResult(result));

            Assert.AreEqual("email@address.com", ex.EmailAddress);
            Assert.AreEqual(
                "Subcriber was excluded. EmailAddress: email@address.com Additional Info: ExactTarget response indicates failure. StatusCode:Error StatusMessage:Reason for failing SubscriberFailures: ErrorCode:123 ErrorDescription:Some other failure| ErrorCode:24 ErrorDescription:Subscriber was excluded by List Detective",
                ex.Message);
        }

        [Test]
        public void CheckResultThrowsExceptionWithSubscriberExcludedErrorAndSubscriberInfoIsNotSupplied()
        {
            var result = new TriggeredSendCreateResult
            {
                StatusCode = "Error",
                StatusMessage = "Reason for failing",
                SubscriberFailures = new[]
                {
                    new SubscriberResult
                    {
                        ErrorCode = "24",
                        ErrorDescription = "Subscriber was excluded by List Detective"
                    }
                }
            };

            var ex = Assert.Throws<SubscriberExcludedException>(() => ExactTargetResultChecker.CheckResult(result));

            Assert.IsNull(ex.EmailAddress);
            Assert.AreEqual(
                "Subcriber was excluded. EmailAddress:  Additional Info: ExactTarget response indicates failure. StatusCode:Error StatusMessage:Reason for failing SubscriberFailures: ErrorCode:24 ErrorDescription:Subscriber was excluded by List Detective",
                ex.Message);
        }

        [Test]
        public void CheckResultThrowsExceptionWithSubscriberExcludedError_When_The_Error_Code_Is_In_The_ErrorDescription
            ()
        {
            var result = new TriggeredSendCreateResult
            {
                StatusCode = "Error",
                StatusMessage = "Reason for failing",
                SubscriberFailures = new[]
                {
                    new SubscriberResult
                    {
                        ErrorCode = "TriggeredSendSubscriberProcessingError",
                        ErrorDescription = "Error Code: 24 - Subscriber was excluded by List Detective.",
                        Subscriber = new Subscriber {EmailAddress = "email@address.com"}
                    }
                }
            };

            var ex = Assert.Throws<SubscriberExcludedException>(() => ExactTargetResultChecker.CheckResult(result));

            Assert.AreEqual("email@address.com", ex.EmailAddress);
            Assert.AreEqual(
                "Subcriber was excluded. EmailAddress: email@address.com Additional Info: ExactTarget response indicates failure. StatusCode:Error StatusMessage:Reason for failing SubscriberFailures: ErrorCode:TriggeredSendSubscriberProcessingError ErrorDescription:Error Code: 24 - Subscriber was excluded by List Detective.",
                ex.Message);
        }
    }
}