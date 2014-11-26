using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RequirementGathering.DAL;
using RequirementGathering.Models;

namespace RequirementGathering.Controllers
{
    public class ResearcherController : BaseController
    {
        // GET: Researcher
        public ActionResult Index()
        {
            var client = from m in RgDbContext.Users select m;
            var clients = client.Where(s => s.UserName.Equals(this.User.Identity.Name.ToString())).FirstOrDefault();
            @ViewBag.Fullname = clients.FullName; @ViewBag.City = clients.City; @ViewBag.Country = clients.Country;
            @ViewBag.Email = clients.Email; @ViewBag.PhoneNumber = clients.PhoneNumber;
            @ViewBag.StreetAddress = clients.StreetAddress;
            var clientRole = clients.Roles.Where(t => t.UserId.Equals(clients.Id)).ToArray();
            var roles = from m in RgDbContext.Roles select m;
            foreach (var role in clientRole)
            {
                string roleid = role.RoleId;
                var rolename = roles.Where(a => a.Id.Equals(roleid)).FirstOrDefault();
                @ViewBag.Roles += rolename.Name + " . ";
            }
            return View();

        }
        public async Task<ActionResult> ViewAttribute(string searchString)
        {
            var evaluations = from m in RgDbContext.Evaluations select m;
            var evaluation = evaluations.Where(m => m.Version.Equals(searchString)).FirstOrDefault();
            var evaluationAttributes = RgDbContext.EvaluationAttributes.Include(e => e.Attribute).Include(e => e.Evaluation);
            var arrayeva = evaluations.Where(m => m.IsActive.Equals(true)).ToArray();
            List<SelectListItem> dropdownItems = new List<SelectListItem>();
            for (int i = 0; i < arrayeva.Length; i++)
            {
                dropdownItems.AddRange(new[]{
                            new SelectListItem() { Text = arrayeva[i].Version, Value = arrayeva[i].Version }});
            }
            ViewData.Add("DropDownItems", dropdownItems);
            if (searchString != "" && searchString != null)
            {
                evaluationAttributes = RgDbContext.EvaluationAttributes.Include(e => e.Attribute).Include(e => e.Evaluation).Where(s => s.Evaluation.Version.Equals(evaluation.Version));
                return View(await evaluationAttributes.ToListAsync());
            }
            else
            {
                return View(await evaluationAttributes.ToListAsync());
            }
        }
        public async Task<ActionResult> ViewUser(string EvaluationVersion, string Username)
        {
            ViewBag.EvaluationVersion = new SelectList(RgDbContext.Evaluations, "Id", "Version");
            if (EvaluationVersion != null  && Username!=null)
            {
                if (EvaluationVersion == "All")
                {
                    var evaluationUsers = RgDbContext.EvaluationUsers.Include(e => e.Evaluation).Include(e => e.User).Where(m => m.User.FirstName.Contains(Username));
                    @ViewBag.numUser = evaluationUsers.Count(); @ViewBag.numUserActive = evaluationUsers.Where(m=>m.IsActive.Equals(true)).Count();
                    return View(await evaluationUsers.ToListAsync());
                }
                else
                {
                    int ID = int.Parse(EvaluationVersion);
                    var evaluationUsers = RgDbContext.EvaluationUsers.Include(e => e.Evaluation).Include(e => e.User).Where(m => m.EvaluationId.Equals(ID)).Where(m => m.User.FirstName.Contains(Username));
                    @ViewBag.numUser = evaluationUsers.Count(); @ViewBag.numUserActive = evaluationUsers.Where(m => m.IsActive.Equals(true)).Count();
                    return View(await evaluationUsers.ToListAsync());
                }
            }
            else
            {
                var evaluationUsers = RgDbContext.EvaluationUsers.Include(e => e.Evaluation).Include(e => e.User);
                @ViewBag.numUser = evaluationUsers.Count(); @ViewBag.numUserActive = evaluationUsers.Where(m => m.IsActive.Equals(true)).Count();
                return View(await evaluationUsers.ToListAsync());
            }
        }

    }
}
