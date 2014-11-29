using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using RequirementGathering.Models;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        public ManageController()
        { }

        public ManageController(ApplicationUserManager userManager)
            : base(userManager)
        { }

        //
        // GET: /Manage/Index
        public ActionResult Index()
        {
            return RedirectToAction("EditProfile");
        }

        //
        // GET: /Manage/EditProfile
        public async Task<ActionResult> EditProfile()
        {
            return View(await GetCurrentUser());
        }

        //
        // POST: /Manage/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                var actualUser = await GetCurrentUser();

                actualUser.Age = user.Age;
                actualUser.City = user.City;
                actualUser.CompanyName = user.CompanyName;
                actualUser.Country = user.Country;
                actualUser.Designation = user.Designation;
                actualUser.District = user.District;
                actualUser.FirstName = user.FirstName;
                actualUser.LastName = user.LastName;
                actualUser.OrganizationName = user.OrganizationName;
                actualUser.PhoneNumber = user.PhoneNumber;
                actualUser.PostalCode = user.PostalCode;
                actualUser.Province = user.Province;
                actualUser.Street = user.Street;

                var result = await UserManager.UpdateAsync(actualUser);

                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "Home", new { Message = FlashMessageId.UpdateProfile });
                }

                AddErrors(result);
            }

            return View(user);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await GetCurrentUser();
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("ChangePassword", new { Message = FlashMessageId.ChangePassword });
            }
            AddErrors(result);
            return View(model);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion
    }
}
