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

        [Range(18, 100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRangeMinMax")]
        [Display(Name = "AgeDisplay", ResourceType = typeof(Resources))]
        public int? Age { get; set; }

        [Display(Name = "OrganizationNameDisplay", ResourceType = typeof(Resources))]
        public string OrganizationName { get; set; }

        [Display(Name = "DesignationDisplay", ResourceType = typeof(Resources))]
        public string Designation { get; set; }

        [Display(Name = "CompanyNameDisplay", ResourceType = typeof(Resources))]
        public string CompanyName { get; set; }

        [Display(Name = "PostalCodeDisplay", ResourceType = typeof(Resources))]
        public string PostalCode { get; set; }

        [Display(Name = "StreetDisplay", ResourceType = typeof(Resources))]
        public string Street { get; set; }

        [Display(Name = "DistrictDisplay", ResourceType = typeof(Resources))]
        public string District { get; set; }

        [Display(Name = "CityDisplay", ResourceType = typeof(Resources))]
        public string City { get; set; }

        [Display(Name = "ProvinceDisplay", ResourceType = typeof(Resources))]
        public string Province { get; set; }

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

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<EvaluationUser> EvaluationUsers { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        #endregion

        public ICollection<Evaluation> InvitedEvaluations()
        {
            return EvaluationUsers.Select(eu => eu.Evaluation)
                                  .Where(eu => eu.IsActive)
                                  .ToList();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
