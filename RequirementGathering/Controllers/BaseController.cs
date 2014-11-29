using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RequirementGathering.DAL;
using RequirementGathering.Helpers;
using RequirementGathering.Models;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Controllers
{
    public class BaseController : Controller
    {
        protected RequirementGatheringDbContext RgDbContext = new RequirementGatheringDbContext();

        public BaseController()
        {
            if (HttpContext != null)
            {
                UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public BaseController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        protected async Task<User> GetCurrentUser()
        {
            return await UserManager.FindByIdAsync(User.Identity.GetUserId());
        }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        private static ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                if (_userManager == null)
                {
                    _userManager = value;
                }
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            SetStatusMessage();

            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        private void SetStatusMessage()
        {
            string param;

            try
            {
                param = Request.Params["Message"];

                if (param == null)
                {
                    return;
                }
            }
            catch (HttpException)
            {
                return;
            }

            FlashMessageId message;

            Enum.TryParse<FlashMessageId>(param, out message);

            ViewBag.StatusMessage =
                      message == FlashMessageId.ChangePassword ? Resources.ChangePassword
                    : message == FlashMessageId.UpdateProfile ? Resources.UpdateProfile
                    : message == FlashMessageId.CreateUser ? Resources.CreateUser
                    : message == FlashMessageId.UpdateUser ? Resources.UpdateUser
                    : message == FlashMessageId.CreateEvaluation ? Resources.CreateEvaluation
                    : message == FlashMessageId.UpdateEvaluation ? Resources.UpdateEvaluation
                    : message == FlashMessageId.CreateProduct ? Resources.CreateProduct
                    : message == FlashMessageId.UpdateProduct ? Resources.UpdateProduct
                    : message == FlashMessageId.AddRating ? Resources.AddRating
                    : message == FlashMessageId.InvitationSent ? Resources.InvitationSent
                    : "";
        }

        public enum FlashMessageId
        {
            ChangePassword,
            CreateUser,
            UpdateUser,
            CreateEvaluation,
            UpdateEvaluation,
            CreateProduct,
            UpdateProduct,
            UpdateProfile,
            AddRating,
            InvitationSent
        }
    }
}
