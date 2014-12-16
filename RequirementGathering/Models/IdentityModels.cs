using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [MinLength(2, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMin")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMax")]
        [Display(Name = "FirstNameDisplay", ResourceType = typeof(Resources))]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMax")]
        [Display(Name = "LastNameDisplay", ResourceType = typeof(Resources))]
        public string LastName { get; set; }

        //[Range(18, 100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRangeMinMax")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(Name = "DateOfBirthDisplay", ResourceType = typeof(Resources))]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "DesignationDisplay", ResourceType = typeof(Resources))]
        public string Designation { get; set; }

        [Display(Name = "CountryDisplay", ResourceType = typeof(Resources))]
        public string Country { get; set; }

        [Display(Name = "NameDisplay", ResourceType = typeof(Resources))]
        public virtual string FullName
        {
            get { return string.Format(CultureInfo.CurrentCulture, "{0} {1}", FirstName, LastName); }
        }

        [Display(Name = "RolesDisplay", ResourceType = typeof(Resources))]
        public virtual string UserRoles { get; set; }

        #region Navigation Fields

        public virtual ICollection<EvaluationUser> EvaluationUsers { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        #endregion

        public ICollection<Evaluation> InvitedEvaluations()
        {
            return EvaluationUsers.Where(eu => eu.IsActive && eu.Evaluation.IsActive && eu.Evaluation.Product.IsActive)
                                  .Select(eu => eu.Evaluation).ToList();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public int Age
        {
            get { return (int)((double)(DateTime.UtcNow - DateOfBirth).Days / 365.2425) + 1; }
        }
    }

    public class EditProfileViewModel
    {
        public ChangePasswordViewModel ChangePassword { get; set; }
        public User User { get; set; }
    }
}
