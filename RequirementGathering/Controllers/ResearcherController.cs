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
        public async Task<ActionResult> Index()
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
