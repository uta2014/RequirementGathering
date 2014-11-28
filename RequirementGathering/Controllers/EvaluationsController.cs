using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name");
            return View(new Evaluation() { Owner = await UserManager.FindByIdAsync(User.Identity.GetUserId()), Attributes = new List<Attribute> { new Attribute() } });
        }

        // POST: Evaluations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Researcher,Administrator,Super Administrator")]
        public async Task<ActionResult> Create(Evaluation evaluation)
        {
            if (evaluation.Attributes == null || evaluation.Attributes.Count < 2)
            {
                ModelState.AddModelError("Attributes", Resources.AttributesCountValidation);
            }

            if (ModelState.IsValid)
            {
                RgDbContext.Evaluations.Add(evaluation);
                await RgDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            if (evaluation.Attributes == null || !evaluation.Attributes.Any())
            {
                evaluation.Attributes = new List<Attribute> { new Attribute() };
            }

            ViewBag.EvaluationIsFreezed = evaluation.EvaluationUsers != null && evaluation.EvaluationUsers.Any();
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name", evaluation.ProductId);
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

            ViewBag.EvaluationIsFreezed = evaluation.EvaluationUsers != null && evaluation.EvaluationUsers.Any();
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name", evaluation.ProductId);
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
            if (evaluation.Attributes == null || evaluation.Attributes.Count < 2)
            {
                ModelState.AddModelError("Attributes", Resources.AttributesCountValidation);
            }

            if (ModelState.IsValid)
            {
                if (evaluation.EvaluationUsers == null || !evaluation.EvaluationUsers.Any())
                {
                    var attributes = RgDbContext.Attributes.Where(a => a.EvaluationId == evaluation.Id);

                    if (attributes.Any())
                    {
                        RgDbContext.Attributes.RemoveRange(attributes);
                    }

                    foreach (var attribute in evaluation.Attributes.Where(a => !string.IsNullOrEmpty(a.Name)))
                    {
                        RgDbContext.Attributes.Add(new Attribute { Name = attribute.Name, EvaluationId = evaluation.Id });
                    }

                    evaluation.Owner = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    evaluation.Attributes.Clear();
                }
                else // Safety
                {
                    evaluation.Attributes = null;
                }

                RgDbContext.Entry(evaluation).State = EntityState.Modified;
                await RgDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            if (evaluation.Attributes == null || !evaluation.Attributes.Any())
            {
                evaluation.Attributes = new List<Attribute> { new Attribute() };
            }

            ViewBag.EvaluationIsFreezed = evaluation.EvaluationUsers != null && evaluation.EvaluationUsers.Any();
            ViewBag.ProductId = new SelectList(RgDbContext.Products, "Id", "Name", evaluation.ProductId);
            return View(evaluation);
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

            return RedirectToAction("Index", "Evaluations");
        }

    }
}
