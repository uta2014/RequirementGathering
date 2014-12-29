using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RequirementGathering.Models;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        public ManageController()
        { }

        public ManageController(ApplicationUserManager userManager)
            : base(userManager)
        { }

        //
        // GET: /Manage/Index
        public ActionResult Index()
        {
            return RedirectToAction("EditProfile");
        }

        //
        // GET: /Manage/EditProfile
        public async Task<ActionResult> EditProfile()
        {
            var user = await GetCurrentUser();
            var profileViewModel = new EditProfileViewModel { User = user };

            GetLists(user);

            return View(profileViewModel);
        }

        //
        // POST: /Manage/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileViewModel profile)
        {
            DateTime time;
            if (DateTime.TryParse(Request.Params["User.DateOfBirth"], out time))
            {
                profile.User.DateOfBirth = time;

                if (profile.User.Age < 18 || profile.User.Age > 100)
                {
                    ModelState.AddModelError("User.DateOfBirth", string.Format(Resources.FieldRangeMinMax, Resources.DateOfBirthDisplay, 18, 100));
                }
            }
            else
            {
                ModelState.AddModelError("User.DateOfBirth", Resources.FieldRangeMinMax);
            }

            if (ModelState.Where(e => !e.Key.StartsWith("ChangePassword.", StringComparison.InvariantCulture)).All(e => !e.Value.Errors.Any()))
            {
                var actualUser = await GetCurrentUser();

                actualUser.DateOfBirth = profile.User.DateOfBirth;
                actualUser.Country = profile.User.Country;
                actualUser.Designation = profile.User.Designation;
                actualUser.FirstName = profile.User.FirstName;
                actualUser.LastName = profile.User.LastName;
                actualUser.Occupation = profile.User.Occupation;
                actualUser.Sex = profile.User.Sex;
                actualUser.Education = profile.User.Education;

                var result = await UserManager.UpdateAsync(actualUser);

                if (result.Succeeded)
                {
                    if (Request.Params["hasChangedPassword"] == "false")
                    {
                        return RedirectToAction("Dashboard", "Home", new { Message = FlashMessageId.UpdateProfile });
                    }
                    else
                    {
                        result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), profile.ChangePassword.OldPassword, profile.ChangePassword.NewPassword);

                        if (result.Succeeded)
                        {
                            var user = await GetCurrentUser();
                            if (user != null)
                            {
                                await SignInAsync(user, isPersistent: false);
                            }

                            return RedirectToAction("Dashboard", "Home", new { Message = FlashMessageId.UpdateProfile });
                        }
                    }
                }

                AddErrors(result);
            }

            GetLists(profile.User);
            return View(profile);
        }

        // POST: /Account/Export
        [HttpPost]
        [Authorize(Roles = "Administrator,Super Administrator,Researcher")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Export(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", Resources.EvaluationIdNull);
                return RedirectToAction("Dashboard", "Home");
            }

            Evaluation evaluation = await RgDbContext.Evaluations.FindAsync(id);

            if (evaluation == null)
            {
                ModelState.AddModelError("", Resources.EvaluationNotFound);
                return RedirectToAction("Dashboard", "Home");
            }

            if (!evaluation.Product.IsActive)
            {
                ModelState.AddModelError("", Resources.EvaluationInactive);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                RespondWithExcelFile(evaluation);

                return View();
            }
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void RespondWithExcelFile(Evaluation evaluation)
        {
            ExcelPackage package = new ExcelPackage();

            foreach (var evaluationUser in evaluation.EvaluationUsers.Where(eu => !eu.IsActive))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(evaluationUser.User.FullName);

                ApplyStyles(worksheet);
                PopulateMeta(evaluationUser, worksheet);
                PopulateRatings(evaluationUser, worksheet);
                worksheet.Cells.AutoFitColumns();
            }

            if (package.Workbook.Worksheets.Count == 0)
            {
                package.Workbook.Worksheets.Add("Empty");
            }

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=WebStreamDownload.xlsx");
            Response.BinaryWrite(package.GetAsByteArray());
            Response.End();
        }

        private void ApplyStyles(ExcelWorksheet worksheet)
        {
            var range1 = worksheet.Cells["A7:B7"];

            range1.Merge = true;
            range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range1.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(102, 102, 102));
            range1.Style.Font.Color.SetColor(Color.FromArgb(238, 238, 238));
            range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var range1a = worksheet.Cells["A7:B12"];
            range1a.Style.Border.BorderAround(ExcelBorderStyle.Thick);

            var range2 = worksheet.Cells["A15:B15"];

            range2.Merge = true;
            range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range2.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(102, 102, 102));
            range2.Style.Font.Color.SetColor(Color.FromArgb(238, 238, 238));
            range2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var range2a = worksheet.Cells["A15:B23"];
            range2a.Style.Border.BorderAround(ExcelBorderStyle.Thick);

            worksheet.Cells["B17"].Style.Numberformat.Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            worksheet.Cells["B23"].Style.Numberformat.Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        private void PopulateMeta(EvaluationUser evaluationUser, ExcelWorksheet worksheet)
        {
            worksheet.Cells["A7"].Value = string.Format("{0} {1}", Resources.Evaluation, Resources.Details);
            worksheet.Cells["A8"].Value = Resources.Product;
            worksheet.Cells["B8"].Value = evaluationUser.Evaluation.Product.Name;
            worksheet.Cells["A9"].Value = Resources.VersionDisplay;
            worksheet.Cells["B9"].Value = evaluationUser.Evaluation.Version;
            worksheet.Cells["A10"].Value = Resources.DescriptionDisplay;
            worksheet.Cells["B10"].Value = evaluationUser.Evaluation.Description;
            worksheet.Cells["A11"].Value = Resources.Steps;
            worksheet.Cells["B11"].Value = evaluationUser.Evaluation.Steps;
            worksheet.Cells["A12"].Value = Resources.Language;
            worksheet.Cells["B12"].Value = evaluationUser.EvaluationLanguage;

            worksheet.Cells["A15"].Value = string.Format("{0} {1}", Resources.Evaluator, Resources.Details);
            worksheet.Cells["A16"].Value = Resources.NameDisplay;
            worksheet.Cells["B16"].Value = evaluationUser.User.FullName;
            worksheet.Cells["A17"].Value = Resources.DateOfBirthDisplay;
            worksheet.Cells["B17"].Value = evaluationUser.User.DateOfBirth;
            worksheet.Cells["A18"].Value = Resources.SexDisplay;
            worksheet.Cells["B18"].Value = string.IsNullOrEmpty(evaluationUser.User.Sex) ? string.Empty : Resources.ResourceManager.GetString(evaluationUser.User.Sex);
            worksheet.Cells["A19"].Value = Resources.PhoneNumberDisplay;
            worksheet.Cells["B19"].Value = evaluationUser.User.PhoneNumber;
            worksheet.Cells["A20"].Value = Resources.DesignationDisplay;
            worksheet.Cells["B20"].Value = evaluationUser.User.Designation;
            worksheet.Cells["A21"].Value = Resources.OccupationDisplay;
            worksheet.Cells["B21"].Value = string.IsNullOrEmpty(evaluationUser.User.Occupation) ? string.Empty : Resources.ResourceManager.GetString(evaluationUser.User.Occupation);
            worksheet.Cells["A22"].Value = Resources.CountryDisplay;
            worksheet.Cells["B22"].Value = evaluationUser.User.Country;
            worksheet.Cells["A23"].Value = Resources.DateTaken;
            worksheet.Cells["B23"].Value = evaluationUser.DateModified;
        }

        private void PopulateRatings(EvaluationUser evaluationUser, ExcelWorksheet worksheet)
        {
            var attributeArray = evaluationUser.Evaluation.Attributes.ToArray();
            int startIndexRow = 6;
            int startIndexColumn = 5;
            int attributeCount = attributeArray.Length;
            var range = worksheet.Cells["E5:G5"];

            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(102, 102, 102));
            range.Style.Font.Color.SetColor(Color.FromArgb(238, 238, 238));
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Merge = true;
            range.Value = Resources.Ratings;

            for (int i = 0; i < attributeCount; ++i)
            {
                var currentRow = startIndexRow + i;
                var currentColumn = startIndexColumn + i + 1;
                var currentAttribute = attributeArray[i];
                ExcelRichText ert = worksheet.Cells[startIndexRow, currentColumn].RichText.Add(currentAttribute.Name);

                ert.Bold = true;

                worksheet.Cells[startIndexRow, currentColumn].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                currentRow++;

                ert = worksheet.Cells[currentRow, startIndexColumn].RichText.Add(currentAttribute.Name);
                ert.Bold = true;

                worksheet.Cells[currentRow, startIndexColumn].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            startIndexRow++;
            startIndexColumn++;

            for (int i = 0; i < attributeCount; ++i)
            {
                for (int j = i; j >= 0; --j)
                {
                    if (attributeArray[j] == attributeArray[i])
                    {
                        worksheet.Cells[startIndexRow + i, startIndexColumn + j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[startIndexRow + i, startIndexColumn + j].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(120, 120, 120));
                        continue;
                    }

                    var attribute = attributeArray[j];
                    var rating = evaluationUser.Ratings.FirstOrDefault(r =>
                        (r.Attribute1 == attributeArray[i] && r.Attribute2 == attributeArray[j]) ||
                        (r.Attribute1 == attributeArray[j] && r.Attribute2 == attributeArray[i]));

                    if (rating == null)
                    {
                        continue;
                    }

                    var currentRow = startIndexRow + i;
                    var currentColumn = startIndexColumn + j;

                    worksheet.Cells[currentRow, currentColumn].Value = rating.Value;
                    worksheet.Cells[currentRow, currentColumn].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }
        }


        private class Country
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }

        private void GetLists(User user)
        {
            ViewData["User.Sex"] = new SelectList(new dynamic[] {
                new {name = Resources.Male, value = "Male"},
                new {name = Resources.Female, value = "Female"},
                new {name = Resources.Other, value = "Other"}
            }, "value", "name", user.Sex);
            ViewData["User.Education"] = new SelectList(new dynamic[] {
                new {name = Resources.EducationBasicLevel, value = "EducationBasicLevel"},
                new {name = Resources.EducationUpperSecondaryLevel, value = "EducationUpperSecondaryLevel"},
                new {name = Resources.EducationBachelorLevel, value = "EducationBachelorLevel"},
                new {name = Resources.EducationMasterLevel, value = "EducationMasterLevel"}
            }, "value", "name", user.Education);
        }
        #endregion
    }
}
