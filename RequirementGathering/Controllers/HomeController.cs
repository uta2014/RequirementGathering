using System.Web.Mvc;

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
        public ActionResult Dashboard()
        {
            ViewBag.Message = "Your dashboard page.";

            return View("Dashboard", "~/Views/Shared/_AuthorizedLayout.cshtml");
        }
    }
}
