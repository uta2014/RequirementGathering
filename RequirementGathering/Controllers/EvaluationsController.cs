using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RequirementGathering.Helpers;
using RequirementGathering.Models;
using RequirementGathering.Reousrces;
using Attribute = RequirementGathering.Models.Attribute;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class EvaluationsController : BaseController
    {
        // GET: Evaluations
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Index()
        {
            return View(await RgDbContext.Evaluations.ToListAsync());
        }

        // GET: Evaluations/Details/5
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
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
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Create()
        {
            ViewBag.EvaluationIsFreezed = false;
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "CulturedName");
            return View(new Evaluation() { Owner = await GetCurrentUser(), Attributes = new List<Attribute> { new Attribute() } });
        }

        // POST: Evaluations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Create(Evaluation evaluation)
        {
            evaluation.Attributes = evaluation.Attributes.Where(a => !string.IsNullOrEmpty(a.Name))
                                      .Distinct(new AttributesIgnoreCaseComparer(CultureInfo.CurrentCulture)).ToList();

            if (evaluation.Attributes == null || evaluation.Attributes.Count < 2)
            {
                ModelState.AddModelError("Attributes", Resources.AttributesCountValidation);
            }

            if (await RgDbContext.Evaluations.AnyAsync(e =>
                e.ProductId == evaluation.ProductId &&
                e.Version.Equals(evaluation.Version, StringComparison.InvariantCultureIgnoreCase)))
            {
                ModelState.AddModelError("", Resources.VersionConflict);
            }

            evaluation.ImageUrl = UploadImage();

            if (ModelState.IsValid)
            {
                RgDbContext.Evaluations.Add(evaluation);
                await RgDbContext.SaveChangesAsync();

                return RedirectToAction("Index", new { Message = FlashMessageId.CreateEvaluation });
            }

            if (evaluation.Attributes == null || !evaluation.Attributes.Any())
            {
                evaluation.Attributes = new List<Attribute> { new Attribute() };
            }

            ViewBag.EvaluationIsFreezed = !CanUpdateAttributes(evaluation);
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "CulturedName", evaluation.ProductId);
            return View(evaluation);
        }

        // GET: Evaluations/Edit/5
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Edit(int? id)
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

            if (evaluation.Attributes == null || !evaluation.Attributes.Any())
            {
                evaluation.Attributes = new List<Attribute> { new Attribute() };
            }

            ViewBag.EvaluationIsFreezed = !CanUpdateAttributes(evaluation);
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "CulturedName", evaluation.ProductId);
            return View(evaluation);
        }

        // POST: Evaluations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Edit(Evaluation evaluation)
        {
            if (evaluation.Attributes == null)
            {
                ModelState.AddModelError("Attributes", Resources.AttributesCountValidation);
            }
            else
            {
                evaluation.Attributes = evaluation.Attributes.Where(a => !string.IsNullOrEmpty(a.Name))
                                       .Distinct(new AttributesIgnoreCaseComparer(CultureInfo.CurrentCulture)).ToList();

                if (evaluation.Attributes.Count < 2)
                {
                    ModelState.AddModelError("Attributes", Resources.AttributesCountValidation);
                }
            }

            if (await RgDbContext.Evaluations.AnyAsync(e =>
                e.Id != evaluation.Id &&
                e.ProductId == evaluation.ProductId &&
                e.Version.Equals(evaluation.Version, StringComparison.InvariantCultureIgnoreCase)))
            {
                ModelState.AddModelError("", Resources.VersionConflict);
            }

            if (Request.Params["FileRemoved"] == "yes")
            {
                var filePath = Path.GetFullPath(Request.PhysicalApplicationPath + evaluation.ImageUrl);

                if (HasValidImageExtension(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            if (Request.Params["FileRemoved"] != "no")
            {
                evaluation.ImageUrl = UploadImage();
            }

            if (ModelState.IsValid)
            {
                if (CanUpdateAttributes(evaluation))
                {
                    var updatedAttributes = evaluation.Attributes.Where(a => a.Id != null);
                    var toBeDeletedAttributes = RgDbContext.Set<Attribute>().AsNoTracking().Where(a => a.EvaluationId == evaluation.Id).ToList();
                    toBeDeletedAttributes = toBeDeletedAttributes.Where(a => !updatedAttributes.Any(ua => ua.Id == a.Id)).ToList();

                    foreach (var attribute in toBeDeletedAttributes)
                    {
                        RgDbContext.Entry(attribute).State = EntityState.Deleted;
                    }

                    foreach (var attribute in updatedAttributes)
                    {
                        attribute.EvaluationId = evaluation.Id;
                        RgDbContext.Entry(attribute).State = EntityState.Modified;
                    }

                    foreach (var attribute in evaluation.Attributes.Where(a => a.Id == null))
                    {
                        RgDbContext.Attributes.Add(new Attribute { Name = attribute.Name, EvaluationId = evaluation.Id });
                    }

                    var currentUser = await GetCurrentUser();
                    evaluation.OwnerId = currentUser.Id;
                    evaluation.Attributes.Clear();
                }
                else // Safety
                {
                    evaluation.Attributes = null;
                }

                RgDbContext.Entry(evaluation).State = EntityState.Modified;
                await RgDbContext.SaveChangesAsync();

                return RedirectToAction("Index", new { Message = FlashMessageId.UpdateEvaluation });
            }

            if (evaluation.Attributes == null || !evaluation.Attributes.Any())
            {
                evaluation.Attributes = new List<Attribute> { new Attribute() };
            }

            ViewBag.EvaluationIsFreezed = !CanUpdateAttributes(evaluation);
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "CulturedName", evaluation.ProductId);
            return View(evaluation);
        }

        [Authorize]
        public async Task<ActionResult> MyEvaluations()
        {
            var currentUser = await GetCurrentUser();
            return View(currentUser.InvitedEvaluations());
        }

        [Authorize]
        public async Task<ActionResult> EvaluationDescription(int? id)
        {
            var currentUser = await GetCurrentUser();

            if (id == null ||
               !await RgDbContext.EvaluationUsers.AnyAsync(eu => eu.EvaluationId == id && eu.UserId == currentUser.Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var evaluation = await RgDbContext.Evaluations.FindAsync(id);

            if (evaluation == null)
            {
                return RedirectToAction("MyEvaluations", "Evaluations");
            }

            return View(evaluation);
        }

        [Authorize]
        public ActionResult Reports()
        {
            return View();
        }

        // GET: Evaluations/Delete/5
        //[Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);
        //    if (evaluation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(evaluation);
        //}

        // POST: Evaluations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);
        //    RgDbContext.Evaluations.Remove(evaluation);
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

        //
        // GET: /Account/SendInvitation
        [Authorize(Roles = "Administrator,Super Administrator,Researcher")]
        public async Task<ActionResult> SendInvitation(int? id)
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

        //
        // POST: /Account/SendInvitation
        [HttpPost]
        [Authorize(Roles = "Administrator,Super Administrator,Researcher")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendInvitation(int? Id, string email, string firstName, string lastName, string description)
        {
            if (Id == null)
            {
                ModelState.AddModelError("", "Evalution Id cannot be null");
                return View();
            }

            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(Id);

            if (evaluation == null)
            {
                ModelState.AddModelError("", "Evaluation not found");
                return View();
            }

            User user = await UserManager.FindByEmailAsync(email);

            if (user != null)
            {
                EvaluationUser evaluationUser = RgDbContext.EvaluationUsers.FirstOrDefault(eu => eu.UserId == user.Id && eu.EvaluationId == evaluation.Id);

                if (evaluationUser != null)
                {
                    if (!evaluationUser.IsActive)
                        ModelState.AddModelError("", "User has already taken this evaluation");
                    else
                        ModelState.AddModelError("", "User was invited previously to this evaluation");

                    return View();
                }

                RgDbContext.EvaluationUsers.Add(new EvaluationUser { UserId = user.Id, EvaluationId = evaluation.Id });
                await RgDbContext.SaveChangesAsync();

                await UserManager.SendEmailAsync(
                      user.Id,
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailSubject, evaluation.Product.Name, evaluation.Version),
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailBodyExisting, Request.Url.GetLeftPart(UriPartial.Authority), Thread.CurrentThread.CurrentUICulture, description)
                );
            }
            else
            {
                user = new User { FirstName = firstName, LastName = lastName, Email = email, UserName = email };
                string password = PasswordHelper.GeneratePassword();
                IdentityResult result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    RgDbContext.EvaluationUsers.Add(new EvaluationUser { UserId = user.Id, EvaluationId = evaluation.Id });
                    await RgDbContext.SaveChangesAsync();

                    await UserManager.SendEmailAsync(
                      user.Id,
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailSubject, evaluation.Product.Name, evaluation.Version),
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailBodyNew, Request.Url.GetLeftPart(UriPartial.Authority), Thread.CurrentThread.CurrentUICulture, password, description)
                    );
                }
                else
                {
                    ModelState.AddModelError("", "Sorry the user cannot be created, try again later");
                    return View();
                }
            }

            return RedirectToAction("Index", "Evaluations", new { Message = FlashMessageId.ChangePassword });
        }

        #region Helpers
        private string UploadImage()
        {
            if (Request.Files.Count == 0 || !ModelState.IsValid)
            {
                return string.Empty;
            }

            HttpPostedFileBase upload = Request.Files[0];

            if (upload.ContentLength == 0)
            {
                return string.Empty;
            }

            string siteRootRelativeUrlToImagesDir = "~/images/products/";
            string pathToSave = Server.MapPath(siteRootRelativeUrlToImagesDir);
            string filename = Path.GetFileName(upload.FileName);

            if (!HasValidImageExtension(filename))
            {
                ModelState.AddModelError("", Resources.ImageUploadFormat);
                return string.Empty;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var pathFormat = pathToSave + "{0}" + Path.GetExtension(filename);
            var destination = Path.Combine(pathToSave, filename);
            Random rand = new Random();

            // Make sure it doesn't overwrites the existing file
            while (System.IO.File.Exists(destination))
            {
                destination = string.Format(CultureInfo.CurrentCulture, pathFormat, fileNameWithoutExtension + rand.Next());
            }

            upload.SaveAs(destination);

            return Url.Content(siteRootRelativeUrlToImagesDir + Path.GetFileName(destination));
        }

        private bool HasValidImageExtension(string filename)
        {
            return new[] { ".gif", ".jpg", ".jpeg", ".png" }.Any(f => f.Equals(Path.GetExtension(filename), StringComparison.InvariantCultureIgnoreCase));
        }

        private bool CanUpdateAttributes(Evaluation evaluation)
        {
            return (evaluation.EvaluationUsers == null ||
                   !evaluation.EvaluationUsers.Any()) &&
                   !RgDbContext.Attributes.Any(a => a.EvaluationId == evaluation.Id && a.Ratings.Any());
        }
        #endregion

    }
}
