using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RequirementGathering.Helpers;

namespace RequirementGathering
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new SetCultureActionFilterAttribute());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Server.GetLastError() as HttpAntiForgeryException != null)
            {
                Server.ClearError();
                Response.Redirect("~/Login");
            }
        }
    }

    public class SetCultureActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.RouteData.Values["controller"].ToString()
               .Equals("Error", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var response = filterContext.RequestContext.HttpContext.Response;
            var culture = filterContext.RouteData.Values["culture"].ToString();

            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            // Save culture in a cookie
            HttpCookie cookie = filterContext.RequestContext.HttpContext.Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            response.Cookies.Add(cookie);
        }
    }
}
