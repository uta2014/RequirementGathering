using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RequirementGathering.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string StreetAddress { get; set; }

        #region Navigation Fields

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<EvaluationUser> EvaluationUsers { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        #endregion

        public ICollection<Evaluation> InvitedEvaluations()
        {
            return EvaluationUsers.Select(eu => eu.Evaluation)
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
