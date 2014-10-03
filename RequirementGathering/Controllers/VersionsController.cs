using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RequirementGathering.DAL;
using RequirementGathering.Models;

namespace RequirementGathering.Controllers
{
    public class VersionsController : Controller
    {
        private RequirementGatheringDbContext db = new RequirementGatheringDbContext();

        // GET: Versions
        public async Task<ActionResult> Index()
        {
            var versions = db.Versions.Include(v => v.Evaluation);
            return View(await versions.ToListAsync());
        }

        // GET: Versions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Version version = await db.Versions.FindAsync(id);
            if (version == null)
            {
                return HttpNotFound();
            }
            return View(version);
        }

        // GET: Versions/Create
        public ActionResult Create()
        {
            ViewBag.EvaluationId = new SelectList(db.Evaluations, "Id", "Name");
            return View();
        }

        // POST: Versions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,EvaluationId,Number")] Version version)
        {
            if (ModelState.IsValid)
            {
                db.Versions.Add(version);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EvaluationId = new SelectList(db.Evaluations, "Id", "Name", version.EvaluationId);
            return View(version);
        }

        // GET: Versions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Version version = await db.Versions.FindAsync(id);
            if (version == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvaluationId = new SelectList(db.Evaluations, "Id", "Name", version.EvaluationId);
            return View(version);
        }

        // POST: Versions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EvaluationId,Number")] Version version)
        {
            if (ModelState.IsValid)
            {
                db.Entry(version).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EvaluationId = new SelectList(db.Evaluations, "Id", "Name", version.EvaluationId);
            return View(version);
        }

        // GET: Versions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Version version = await db.Versions.FindAsync(id);
            if (version == null)
            {
                return HttpNotFound();
            }
            return View(version);
        }

        // POST: Versions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Version version = await db.Versions.FindAsync(id);
            db.Versions.Remove(version);
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
