using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RequirementGathering.DAL;
using RequirementGathering.Models;

namespace RequirementGathering.Controllers
{
    public class EvaluationsController : Controller
    {
        private RequirementGatheringDbContext db = new RequirementGatheringDbContext();

        // GET: Evaluations
        public async Task<ActionResult> Index()
        {
            return View(await db.Evaluations.ToListAsync());
        }

        // GET: Evaluations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = await db.Evaluations.FindAsync(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // GET: Evaluations/Create
        public ActionResult Create()
        {
            return View(new Evaluation());
        }

        // POST: Evaluations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,IsActive")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                db.Evaluations.Add(evaluation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(evaluation);
        }

        // GET: Evaluations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = await db.Evaluations.FindAsync(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // POST: Evaluations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,IsActive")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(evaluation);
        }

        // GET: Evaluations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = await db.Evaluations.FindAsync(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // POST: Evaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Evaluation evaluation = await db.Evaluations.FindAsync(id);
            db.Evaluations.Remove(evaluation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
