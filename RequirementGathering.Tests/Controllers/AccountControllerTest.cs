using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequirementGathering.Controllers;

namespace RequirementGathering.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        /*
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

        
        [TestMethod]
        public async Task AccountLogin()
        {
            
            IUserStore<User> iu=new UserStore<User>();
            ApplicationUserManager um = new ApplicationUserManager(iu);

            // Arrange
            AccountController controller = new AccountController();

            LoginViewModel lvm = new LoginViewModel();
            lvm.Email = "eija@uta.fi";
            lvm.Password = "DefaultPasscode123!!";
            lvm.RememberMe = false;

            // Act
            ActionResult result = await controller.Login(lvm, null);

            // Assert
            Assert.IsNotNull(result);
            
        }
        

        [TestMethod]
        public async Task AccountEdit()
        {
            AccountController controller = new AccountController();


            ActionResult result = await controller.Edit("ididid");
            Assert.IsNotNull(result);

        }
        */

    }
}
