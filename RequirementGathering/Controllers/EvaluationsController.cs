using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RequirementGathering.Models;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class EvaluationsController : BaseController
    {
        // GET: Evaluations
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Index()
        {
            return View(await RgDbContext.Evaluations.ToListAsync());
        }

        // GET: Evaluations/Details/5
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // GET: Evaluations/Create
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name");
            return View(new Evaluation());
        }

        // POST: Evaluations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Version,Description,IsActive,ProductId")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                RgDbContext.Evaluations.Add(evaluation);
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name", evaluation.ProductId);
            return View(evaluation);
        }

        // GET: Evaluations/Edit/5
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);

            if (evaluation == null)
                return HttpNotFound();

            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name", evaluation.ProductId);
            return View(evaluation);
        }

        // POST: Evaluations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Version,Description,IsActive,ProductId")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                RgDbContext.Entry(evaluation).State = EntityState.Modified;
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(RgDbContext.Products, "ProductId", "Name", evaluation.ProductId);
            return View(evaluation);
        }

        // GET: Evaluations/Delete/5
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // POST: Evaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);
            RgDbContext.Evaluations.Remove(evaluation);
            await RgDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RgDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
