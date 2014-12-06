using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequirementGathering.Controllers;

namespace RequirementGathering.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public async Task AccountIndex()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
