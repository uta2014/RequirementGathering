using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RequirementGathering.Models;
using System.Collections.Generic;
using RequirementGathering.Helpers;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {


        


        
        // GET: Products
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Index()
        {
            var products = RgDbContext.Products.Include(p => p.Owner);
            return View(await products.ToListAsync());
        }


        // GET: Products
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> SortedIndex(string sortname, string sorttype, int? sortSelectedIndex)
        {
            var products = RgDbContext.Products.Include(p => p.Owner);
            List<Product> lp = await products.ToListAsync();
            if (sortname != null && sortname != "" && sorttype != null && sorttype != "")
            {
                lp.Sort(new ProductComparer(sortname, (sorttype.Equals("asc") ? true : false)));
                ViewData["sortSelectedIndex"] = sortSelectedIndex;
            }
            else
            {
                lp.Sort(new ProductComparer("Name", true));
                ViewData["sortSelectedIndex"] = 0;
            }

            
            return View("Index",lp);
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
    }
}
