using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RequirementGathering.Helpers;
using RequirementGathering.Models;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        // GET: Products
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Index(string sort)
        {
            var products = await RgDbContext.Products.Include(p => p.Owner).ToListAsync();

            if (!string.IsNullOrEmpty(sort) && sort.Any(s => s == '_'))
            {
                var sortOptions = sort.Split('_');
                products.Sort(new ProductComparer(sortOptions[0], (sortOptions[1].Equals("asc", StringComparison.InvariantCultureIgnoreCase))));
                ProduceSortingOptions(sort);
            }
            else
            {
                products.Sort(new ProductComparer("Name"));
                ProduceSortingOptions();
            }

            return View(products);
        }

        // GET: Products/Details/5
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await RgDbContext.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Create()
        {
            return View(new Product() { Owner = await GetCurrentUser() });
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,IsActive")] Product product)
        {
            if (ModelState.IsValid)
            {
                RgDbContext.Products.Add(product);
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = FlashMessageId.CreateProduct });
            }

            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await RgDbContext.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,IsActive")] Product product)
        {
            if (ModelState.IsValid)
            {
                RgDbContext.Entry(product).State = EntityState.Modified;
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = FlashMessageId.UpdateProduct });
            }

            return View(product);
        }

        // GET: Products/Delete/5
        //[Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    Product product = await RgDbContext.Products.FindAsync(id);

        //    if (product == null)
        //        return HttpNotFound();

        //    return View(product);
        //}

        // POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Product product = await RgDbContext.Products.FindAsync(id);
        //    RgDbContext.Products.Remove(product);
        //    await RgDbContext.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RgDbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private void ProduceSortingOptions(object selected = null)
        {
            ViewBag.SortOptions = new SelectList(new[] {
                    new {Id = "Name_asc", Name = string.Format("{0} {1}", Resources.NameDisplay, "A-Z")},
                    new {Id = "Name_dec", Name = string.Format("{0} {1}", Resources.NameDisplay, "Z-A")},
                    new {Id = "IsActive_asc", Name = string.Format("{0} {1}", Resources.IsActiveDisplay, "A-Z")},
                    new {Id = "IsActive_dec", Name = string.Format("{0} {1}", Resources.IsActiveDisplay, "Z-A")}
                }, "Id", "Name", selected);
        }
        #endregion
    }
}
