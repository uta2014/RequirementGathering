using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequirementGathering.Controllers;
using RequirementGathering.Models;

namespace RequirementGathering.Tests.Controllers
{
    [TestClass]
    public class EvaluationsControllerTest
    {
        /*
        [TestMethod]
        public async Task EvaluationIndex()
        {
            EvaluationsController ec = new EvaluationsController();
            // Act
            ActionResult result = await ec.Index();
            // Assert
            Assert.IsNotNull(result);

        }*/

        [TestMethod]
        public async Task EvaluationDetails()
        {
            EvaluationsController ec = new EvaluationsController();
            // Act
            ActionResult result = await ec.Details(null);
            // Assert
            Assert.IsNotNull(result);
            
        }

        [TestMethod]
        public async Task EvaluationEdit()
        {
            EvaluationsController ec = new EvaluationsController();
            int? i = null;
            // Act
            ActionResult result = await ec.Edit(i);
            // Assert
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task EvaluationSendInvitation()
        {
            EvaluationsController ec = new EvaluationsController();
            int? i = null;
            // Act
            ActionResult result = await ec.SendInvitation(i);
            // Assert
            Assert.IsNotNull(result);

            // Act
            result = await ec.SendInvitation(i,"","","","");
            // Assert
            Assert.IsNotNull(result);

        }
        
    }
}
