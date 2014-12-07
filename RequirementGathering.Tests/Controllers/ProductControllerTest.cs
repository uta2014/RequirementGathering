using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequirementGathering.Controllers;
using System.Web.Routing;
using System.IO;
using System.Security.Principal;
using RequirementGathering.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Specialized;
using System.Collections;
using System;

namespace RequirementGathering.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public async Task ProductIndex()
        {
            ProductsController pc = new ProductsController();
            // Act
            ActionResult result = await pc.Index();
            // Assert
            Assert.IsNotNull(result);
            
        }

        [TestMethod]
        public async Task ProductDetails()
        {
            ProductsController pc = new ProductsController();
            // Act
            ActionResult result = await pc.Details(-123456);
            // Assert
            Assert.IsNotNull(result);

            // Act
            result = await pc.Details(null);
            // Assert
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task ProductCreate()
        {
            Random ran=new Random();
            int RandKey=ran.Next(10000000,99999999);
            Product p=new Product();
            p.Id=RandKey;
            p.Name="TestProd";
            p.Description="TestProd";
            p.IsActive=true;

            ProductsController pc = new ProductsController();
            // Act
            ActionResult result = await pc.Create(p);
            // Assert
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task ProductEdit()
        {
            ProductsController pc = new ProductsController();
            // Act
            ActionResult result = await pc.Edit(-123456);
            // Assert
            Assert.IsNotNull(result);

        }
    }
}
