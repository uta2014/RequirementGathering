<<<<<<< HEAD
﻿using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RequirementGathering.Models;
=======
﻿using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
>>>>>>> Changed Controllers;

namespace RequirementGathering.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
<<<<<<< HEAD
        public async Task<ActionResult> Dashboard()
        {
            ViewBag.Message = "Your dashboard page.";

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            user.UserRoles = string.Join(", ", UserManager.GetRoles(user.Id));

            return View("Dashboard", "~/Views/Shared/_AuthorizedLayout.cshtml", user);
=======
        public ActionResult Dashboard(string userEmail)
        {
            ViewBag.Message = "Your dashboard page.";
            return View("Dashboard", "~/Views/Shared/_AuthorizedLayout.cshtml");
>>>>>>> Changed Controllers;
        }
    }
}
