using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RequirementGathering.Models;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace RequirementGathering.Controllers
{
    public class UsersController : BaseController
    {
        // GET: Users
        [Authorize(Roles = "Administrator,Super Administrator")]
        public ActionResult Index()
        {

            List<User> allUsers = new List<User>();

            foreach (var uitem in RgDbContext.Users)
            {

                allUsers.Add(uitem);
            }

            ViewData["allUsers"] = allUsers;
            ViewData["userManager"] = this.UserManager;

            return View();
        }

        [Authorize(Roles = "Administrator,Super Administrator")]
        public ActionResult CreateUser(string name,string email,string rolestr)
        {
            var userManager = this.UserManager;

            var e = new User { Email = email, UserName = name };
            userManager.Create(e, "DefaultPasscode!!");
            string[] roles = rolestr.Split(',');
            foreach (var str in roles)
            {
                userManager.AddToRole(e.Id, str);
            }

            RgDbContext.SaveChanges();



            refreshUsers();
            return View();
        }

        [Authorize(Roles = "Administrator,Super Administrator")]
        public ActionResult UpdateUser(string oldid, string oldName, string oldEmail, string name, string email)
        {
            var userManager = this.UserManager;

            User ou=userManager.FindById(oldid);

            ou.Email = email;
            ou.UserName = name;

            RgDbContext.SaveChanges();



            refreshUsers();
            return View();
        }

        private void refreshUsers()
        {
            List<User> allUsers = new List<User>();

            foreach (var uitem in RgDbContext.Users)
            {
                UserManager.GetRoles(uitem.Id);
                foreach (var rt in uitem.Roles)
                {
                    
                }
                allUsers.Add(uitem);
            }

            ViewData["allUsers"] = allUsers;
        }

    }

}