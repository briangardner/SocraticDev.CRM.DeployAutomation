using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using NUnit.Framework;
using SocraticDev.CRM.DeployAutomation.Tasks;
using System.Collections.Generic;

namespace SocraticDev.Crm.DeployAutomation.UnitTests.Tasks
{
    [TestFixture]
    public class EnableBingMapsCommandTests
    {
        private Mock<IOrganizationService> _mockOrgService;
        
        [SetUp]
        public void Setup()
        {
            _mockOrgService = new Mock<IOrganizationService>();
        }

        [Test]
        public void Command_Should_Enable_Bing_When_Disabled()
        {
            var settings = new EnableBingMapsSettings { EnableBingMaps = true };
            var entity = new Entity("organization")
            {
                Attributes =
                {
                    new KeyValuePair<string, object>("enablebingmapsintegration", false)
                }
            };
            _mockOrgService.Setup(x => x.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity>()
                {
                    entity
                }));
            var command = new EnableBingMapsCommand(_mockOrgService.Object, settings);
            command.Execute();

            Assert.AreEqual(true, entity.Attributes["enablebingmapsintegration"]);
            _mockOrgService.Verify(x => x.Update(It.IsAny<Entity>()), Times.Once);
        }

        [Test]
        public void Command_Should_Return_When_Setting_Matches_CRM_Value()
        {
            var settings = new EnableBingMapsSettings { EnableBingMaps = true };
            var entity = new Entity("organization")
            {
                Attributes =
                {
                    new KeyValuePair<string, object>("enablebingmapsintegration", true)
                }
            };
            _mockOrgService.Setup(x => x.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity>()
                {
                    entity
                }));
            var command = new EnableBingMapsCommand(_mockOrgService.Object, settings);
            command.Execute();

            Assert.AreEqual(true, entity.Attributes["enablebingmapsintegration"]);
            _mockOrgService.Verify(x => x.Update(It.IsAny<Entity>()), Times.Never);
        }

        [Test]
        public void Command_Should_Disable_Bing_Maps_When_Enabled()
        {
            var settings = new EnableBingMapsSettings { EnableBingMaps = false };
            var entity = new Entity("organization")
            {
                Attributes =
                {
                    new KeyValuePair<string, object>("enablebingmapsintegration", true)
                }
            };
            _mockOrgService.Setup(x => x.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity>()
                {
                    entity
                }));
            var command = new EnableBingMapsCommand(_mockOrgService.Object, settings);
            command.Execute();

            Assert.AreEqual(false, entity.Attributes["enablebingmapsintegration"]);
            _mockOrgService.Verify(x => x.Update(It.IsAny<Entity>()), Times.Once);
        }

    }
}
