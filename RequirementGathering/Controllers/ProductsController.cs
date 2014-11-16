﻿using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RequirementGathering.Models;
using System.Linq;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        // GET: Products
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Index(string searchString)
        {
            var productss = from m in RgDbContext.Products select m;
            var ky = productss.Where(m => m.Name.Equals(searchString)).FirstOrDefault();
            var products = RgDbContext.Products.Include(p => p.Owner);
            if (searchString != "" && searchString != null)
            {
                products = RgDbContext.Products.Include(p => p.Owner).Where(m => m.Id.Equals(ky.Id));
                return View(await products.ToListAsync());
            }
            else
            {
                return View(await products.ToListAsync());
            }
        }

        // GET: Products/Details/5
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await RgDbContext.Products.FindAsync(id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Create()
        {
            return View(new Product() { Owner = await UserManager.FindByIdAsync(User.Identity.GetUserId()) });
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,IsActive")] Product product)
        {
            if (ModelState.IsValid)
            {
                RgDbContext.Products.Add(product);
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await RgDbContext.Products.FindAsync(id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,IsActive")] Product product)
        {
            if (ModelState.IsValid)
            {
                RgDbContext.Entry(product).State = EntityState.Modified;
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await RgDbContext.Products.FindAsync(id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,SuperAdministrator")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await RgDbContext.Products.FindAsync(id);
            RgDbContext.Products.Remove(product);
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
