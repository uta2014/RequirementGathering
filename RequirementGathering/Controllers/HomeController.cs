using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

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
        public async Task<ActionResult> Dashboard()
        {
            ViewBag.Message = "Your dashboard page.";

            var user = await GetCurrentUser();
            user.UserRoles = string.Join(", ", UserManager.GetRoles(user.Id));

            return View("Dashboard", "~/Views/Shared/_AuthorizedLayout.cshtml", user);
        }
    }
}
