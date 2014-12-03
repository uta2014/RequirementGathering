using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RequirementGathering.Extensions;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Views.Helpers
{
    public static class ApplicationHelpers
    {
        public static string BuildBreadcrumbNavigation(this HtmlHelper helper)
        {
            if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Home" ||
               (helper.ViewContext.RouteData.Values["controller"].ToString() == "Account" &&
                helper.ViewContext.RouteData.Values["action"].ToString() == "Login"))
            {
                return string.Empty;
            }

            StringBuilder breadcrumb = new StringBuilder("<div class=\"breadcrumb\"><li>").Append(helper.ActionLink(Resources.Home, "Index", "Home").ToHtmlString()).Append("</li>");

            var controllerName = helper.ViewContext.RouteData.Values["controller"].ToString();
            var controllerNameLocalized = Resources.ResourceManager.GetString(controllerName.Titleize());
            var actionName = helper.ViewContext.RouteData.Values["action"].ToString();
            var actionNameLocalized = Resources.ResourceManager.GetString(actionName.Titleize());

            breadcrumb.Append("<li>");
            breadcrumb.Append(helper.ActionLink(controllerNameLocalized.Titleize(), "Index", controllerName));
            breadcrumb.Append("</li>");

            if (actionNameLocalized != null && actionName != "Index")
            {
                breadcrumb.Append("<li>");
                breadcrumb.Append(helper.ActionLink(actionNameLocalized.Titleize(), actionName, controllerName));
                breadcrumb.Append("</li>");
            }

            return breadcrumb.Append("</div>").ToString();
        }
    }
}
