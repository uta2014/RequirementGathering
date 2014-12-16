using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using RequirementGathering.Models;
using RequirementGathering.Reousrces;

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
            return View(new EditProfileViewModel { User = await GetCurrentUser() });
        }

        //
        // POST: /Manage/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileViewModel profile)
        {
            DateTime time;
            if (DateTime.TryParse(Request.Params["User.DateOfBirth"], out time))
            {
                profile.User.DateOfBirth = time;

                if (profile.User.Age < 18 || profile.User.Age > 100)
                {
                    ModelState.AddModelError("User.DateOfBirth", string.Format(Resources.FieldRangeMinMax, Resources.DateOfBirthDisplay, 18, 100));
                }
            }
            else
            {
                ModelState.AddModelError("User.DateOfBirth", Resources.FieldRangeMinMax);
            }

            if (ModelState.Where(e => !e.Key.StartsWith("ChangePassword.", StringComparison.InvariantCulture)).All(e => !e.Value.Errors.Any()))
            {
                var actualUser = await GetCurrentUser();

                actualUser.DateOfBirth = profile.User.DateOfBirth;
                actualUser.Country = profile.User.Country;
                actualUser.Designation = profile.User.Designation;
                actualUser.FirstName = profile.User.FirstName;
                actualUser.LastName = profile.User.LastName;

                var result = await UserManager.UpdateAsync(actualUser);

                if (result.Succeeded)
                {
                    if (Request.Params["hasChangedPassword"] == "false")
                    {
                        return RedirectToAction("Dashboard", "Home", new { Message = FlashMessageId.UpdateProfile });
                    }
                    else
                    {
                        result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), profile.ChangePassword.OldPassword, profile.ChangePassword.NewPassword);

                        if (result.Succeeded)
                        {
                            var user = await GetCurrentUser();
                            if (user != null)
                            {
                                await SignInAsync(user, isPersistent: false);
                            }

                            return RedirectToAction("Dashboard", "Home", new { Message = FlashMessageId.UpdateProfile });
                        }
                    }
                }

                AddErrors(result);
            }

            return View(profile);
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
