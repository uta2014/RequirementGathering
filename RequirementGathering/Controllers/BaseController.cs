using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using RequirementGathering.DAL;
using RequirementGathering.Helpers;

namespace RequirementGathering.Controllers
{
    public class BaseController : Controller
    {
        protected RequirementGatheringDbContext RgDbContext = new RequirementGatheringDbContext();

        public BaseController()
        {
            if (HttpContext != null)
                UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public BaseController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
                    _userManager = value;
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
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
    }
}
