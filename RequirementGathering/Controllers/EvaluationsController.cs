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
        public async Task<ActionResult> Index(string sort)
        {
            var evaluations = await RgDbContext.Evaluations.Where(e => e.Product.IsActive).ToListAsync();

            if (!string.IsNullOrEmpty(sort) && sort.Any(s => s == '_'))
            {
                var sortOptions = sort.Split('_');
                evaluations.Sort(new EvaluationComparer(sortOptions[0], (sortOptions[1].Equals("asc", StringComparison.InvariantCultureIgnoreCase))));
                ProduceSortingOptions(sort);
            }
            else
            {
                evaluations.Sort(new EvaluationComparer("Name"));
                ProduceSortingOptions();
            }

            return View(evaluations);
        }

        // GET: Sort Evaluations
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> SortedIndex(string sortname, string sorttype, int? sortSelectedIndex)
        {

            List<Evaluation> lp = await RgDbContext.Evaluations.Where(e => e.Product.IsActive).ToListAsync();
            if (sortname != null && sortname != "" && sorttype != null && sorttype != "")
            {
                lp.Sort(new EvaluationComparer(sortname, (sorttype.Equals("asc") ? true : false)));
                ViewData["sortSelectedIndex"] = sortSelectedIndex;
            }
            else
            {
                lp.Sort(new EvaluationComparer("ProductName", true));
                ViewData["sortSelectedIndex"] = 0;
            }


            return View("Index", lp);
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
            ViewBag.ProductId = new SelectList(RgDbContext.Products.Where(p => p.IsActive), "Id", "CulturedName");
            ViewBag.Steps = new SelectList(new dynamic[] {
                new {name = "3 " + Resources.Steps, value = 3},
                new {name = "5 " + Resources.Steps, value = 5}
            }, "value", "name");
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
            ViewBag.ProductId = new SelectList(RgDbContext.Products.Where(p => p.IsActive), "Id", "CulturedName", evaluation.ProductId);
            ViewBag.Steps = new SelectList(new dynamic[] {
                new {name = "3 " + Resources.Steps, value = 3},
                new {name = "5 " + Resources.Steps, value = 5}
            }, "value", "name", evaluation.Steps);
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
            ViewBag.ProductId = new SelectList(RgDbContext.Products.Where(p => p.IsActive), "Id", "CulturedName", evaluation.ProductId);
            ViewBag.Steps = new SelectList(new dynamic[] {
                new {name = "3 " + Resources.Steps, value = 3},
                new {name = "5 " + Resources.Steps, value = 5}
            }, "value", "name", evaluation.Steps);
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
            ViewBag.ProductId = new SelectList(RgDbContext.Products.Where(p => p.IsActive), "Id", "CulturedName", evaluation.ProductId);
            ViewBag.Steps = new SelectList(new dynamic[] {
                new {name = "3 " + Resources.Steps, value = 3},
                new {name = "5 " + Resources.Steps, value = 5}
            }, "value", "name", evaluation.Steps);
            return View(evaluation);
        }

        public async Task<ActionResult> MyEvaluations()
        {
            var currentUser = await GetCurrentUser();
            return View(currentUser.InvitedEvaluations());
        }

        public async Task<ActionResult> EvaluationDescription(int? id)
        {
            var currentUser = await GetCurrentUser();
            var evaluationUser = RgDbContext.EvaluationUsers.FirstOrDefault(eu => eu.IsActive &&
                                                                            eu.EvaluationId == id &&
                                                                            eu.UserId == currentUser.Id &&
                                                                            eu.Evaluation.IsActive &&
                                                                            eu.Evaluation.Product.IsActive);
            if (id == null || evaluationUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var evaluation = await RgDbContext.Evaluations.FindAsync(id);

            if (evaluation == null)
            {
                return RedirectToAction("MyEvaluations", "Evaluations");
            }

            ViewBag.EvaluationUserId = evaluationUser.Id;

            return View(evaluation);
        }

        //
        // POST: /Account/Ratings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Ratings(IEnumerable<Rating> ratings)
        {
            if (!ratings.Any() || ratings.Any(r => r.Value > 5 || r.Value < 0))
            {
                return RedirectToAction("MyEvaluations");
            }

            foreach (var rating in ratings)
            {
                RgDbContext.Ratings.Add(rating);
            }

            var evaluationUser = RgDbContext.EvaluationUsers.Find(ratings.First().EvaluationUserId);

            evaluationUser.IsActive = false;
            evaluationUser.EvaluationLanguage = CultureInfo.CurrentCulture.EnglishName;
            evaluationUser.DateModified = DateTime.UtcNow;
            evaluationUser.ViewName = Request.Params["ViewName"];

            RgDbContext.Entry(evaluationUser).State = EntityState.Modified;

            await RgDbContext.SaveChangesAsync();

            return RedirectToAction("EditProfile", "Manage", new { Message = FlashMessageId.RatingsSubmitted });
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
                ModelState.AddModelError("", Resources.EvaluationIdNull);
                return RedirectToAction("Dashboard", "Home");
            }

            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(Id);

            if (evaluation == null)
            {
                ModelState.AddModelError("", Resources.EvaluationNotFound);
                return RedirectToAction("Dashboard", "Home");
            }

            if (!evaluation.IsActive || !evaluation.Product.IsActive)
            {
                ModelState.AddModelError("", Resources.EvaluationInactive);
                return RedirectToAction("Dashboard", "Home");
            }

            User user = await UserManager.FindByEmailAsync(email);

            if (user != null)
            {
                EvaluationUser evaluationUser = RgDbContext.EvaluationUsers.FirstOrDefault(eu => eu.UserId == user.Id && eu.EvaluationId == evaluation.Id);

                if (evaluationUser != null)
                {
                    if (!evaluationUser.IsActive)
                        ModelState.AddModelError("", Resources.UserTakenEvaluation);
                    else
                        ModelState.AddModelError("", Resources.UserAlreadyInvited);

                    return View(evaluation);
                }

                RgDbContext.EvaluationUsers.Add(new EvaluationUser { UserId = user.Id, EvaluationId = evaluation.Id, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow });
                await RgDbContext.SaveChangesAsync();

                await UserManager.SendEmailAsync(
                      user.Id,
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailSubject, evaluation.Product.Name, evaluation.Version),
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailBodyExisting, Request.Url.GetLeftPart(UriPartial.Authority), Thread.CurrentThread.CurrentUICulture, description)
                );
            }
            else
            {
                user = new User { FirstName = firstName, LastName = lastName, Email = email, UserName = email, DateOfBirth = DateTime.UtcNow.AddYears(-18) };
                string password = PasswordHelper.GeneratePassword();
                IdentityResult result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Evaluator");
                    await UserManager.SendEmailAsync(
                      user.Id,
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailSubject, evaluation.Product.Name, evaluation.Version),
                      string.Format(CultureInfo.CurrentCulture, Resources.InvitationEmailBodyNew, Request.Url.GetLeftPart(UriPartial.Authority), Thread.CurrentThread.CurrentUICulture, password, description)
                    );

                    RgDbContext.EvaluationUsers.Add(new EvaluationUser { UserId = user.Id, EvaluationId = evaluation.Id, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow });
                    await RgDbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", Resources.UserNotCreated);
                    return View(evaluation);
                }
            }

            return RedirectToAction("Index", "Evaluations", new { Message = FlashMessageId.InvitationSent });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RgDbContext.Dispose();
            }
            base.Dispose(disposing);
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

        private void ProduceSortingOptions(object selected = null)
        {
            ViewBag.SortOptions = new SelectList(new[] {
                    new {Id = "ProductName_asc", Name=Resources.NameDisplay +" Asc"},
                    new {Id = "ProductName_dec", Name=Resources.NameDisplay + " Dec"},
                    new {Id = "IsActive_asc", Name=Resources.IsActiveDisplay + " Asc"},
                    new {Id = "IsActive_dec", Name=Resources.IsActiveDisplay + " Dec"}
                }, "Id", "Name", selected);
        }
        #endregion

    }
}
