using System;
using System.Collections.Generic;
using ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension;
using ExactTarget.TriggeredEmail.Core.RequestClients.DeliveryProfile;
using ExactTarget.TriggeredEmail.Core.RequestClients.Email;
using ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition;
using ExactTarget.TriggeredEmail.Creation.Creators;
using Moq;
using NUnit.Framework;

namespace ExactTarget.TriggeredEmail.Test.Unit.Creation.Creators
{
    [TestFixture]
    public class PasteHtmlEmailCreatorTests
    {
        private Mock<IDataExtensionClient> _dataExtensionClient;
        private Mock<IDeliveryProfileClient> _deliveryProfileClient;
        private Mock<IEmailRequestClient> _emailRequestClient;
        private Mock<ITriggeredSendDefinitionClient> _triggeredSendDefinitionClient;

        [SetUp]
        public void SetUp()
        {
            _dataExtensionClient = new Mock<IDataExtensionClient>();
            _emailRequestClient = new Mock<IEmailRequestClient>();
            _triggeredSendDefinitionClient = new Mock<ITriggeredSendDefinitionClient>();
            _deliveryProfileClient = new Mock<IDeliveryProfileClient>();


            _dataExtensionClient.Setup(d => d.RetrieveTriggeredSendDataExtensionTemplateObjectId())
                .Returns(Guid.NewGuid().ToString);
        }

        private PasteHtmlEmailCreator ManufactureCreator()
        {
            return new PasteHtmlEmailCreator(_dataExtensionClient.Object, 
                            _triggeredSendDefinitionClient.Object,
                            _emailRequestClient.Object, 
                            _deliveryProfileClient.Object);
        }

        [Test]
        public void When_Creating_An_Email_A_DataExtension_With_Custom_Fields_Is_Created()
        {
            var creator = ManufactureCreator();
            creator.Create("external-key", "%%value1%% %%value2%%");

            _dataExtensionClient.Verify(d => d.CreateDataExtension(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    "triggeredsend-external-key",
                    It.Is<HashSet<string>>(values => 
                        values.Contains("value1") 
                        && values.Contains("value2"))), 
                Times.Once);
        }

        [Test]
        public void When_Creating_An_Email_A_DataExtension_With_Subject_Field_Is_Created()
        {
            var creator = ManufactureCreator();
            creator.Create("external-key", "<p>email markup</p>");

            _dataExtensionClient.Verify(d => d.CreateDataExtension(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    "triggeredsend-external-key",
                    It.Is<HashSet<string>>(values => values.Contains("Subject"))),
                Times.Once);
        }

        [Test]
        public void When_ExternalKey_Is_Too_Long_Then_An_ArgumentException_Is_Thrown()
        {
            const int maxLength = 36;
            var creator = ManufactureCreator();
            Assert.Throws<ArgumentException>(() => creator.Create("-".PadLeft(maxLength + 1), "<p>email markup</p>"));
        }

        [Test]
        public void When_ExternalKey_Is_Max_Length_Or_Shorter_Then_No_ArgumentException_Is_Thrown()
        {
            const int maxLength = 36;
            var creator = ManufactureCreator();
            Assert.DoesNotThrow(() => creator.Create("-".PadLeft(maxLength), "<p>email markup</p>"));
            Assert.DoesNotThrow(() => creator.Create("-".PadLeft(maxLength - 1), "<p>email markup</p>"));
        }

        [Test]
        public void When_TriggeredSendDefinition_Exists_Then_Exception_Is_Thrown()
        {
            _triggeredSendDefinitionClient.Setup(d => d.DoesTriggeredSendDefinitionExist(It.IsAny<string>())).Returns(true);

            var creator = ManufactureCreator();

            var ex = Assert.Throws<Exception>(() => creator.Create("external-key123", "<p>email markup</p>"));

            Assert.That(ex.Message, Is.StringContaining("A TriggeredSendDefinition with external key external-key123 already exsits"));
        }
    }
}
