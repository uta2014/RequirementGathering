using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RequirementGathering.Models;
using Attribute = RequirementGathering.Models.Attribute;

namespace RequirementGathering.DAL
{
    public class RequirementGatheringInitializer : DropCreateDatabaseIfModelChanges<RequirementGatheringDbContext>
    {
        protected override void Seed(RequirementGatheringDbContext context)
        {
            int result;

            // Seed Roles
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var suRole = new IdentityRole { Name = "Super Administrator" };
            var adminRole = new IdentityRole { Name = "Administrator" };
            var researcherRole = new IdentityRole { Name = "Researcher" };
            var evaluatorRole = new IdentityRole { Name = "Evaluator" };

            context.Roles.Add(suRole);
            context.Roles.Add(adminRole);
            context.Roles.Add(researcherRole);
            context.Roles.Add(evaluatorRole);

            // Seed Users
            var dateOfBirth = DateTime.UtcNow.AddYears(-18);

            var admin = new User { FirstName = "Admin", Email = "admin@uta.fi", UserName = "admin", DateOfBirth = dateOfBirth };

            var users = new List<User>
            {
                admin
            };

            var hasher = new PasswordHasher();

            foreach (var user in users)
            {
                user.PasswordHash = hasher.HashPassword("DefaultPasscode123!!");
                user.EmailConfirmed = true;
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.Roles.Add(new IdentityUserRole { RoleId = researcherRole.Id, UserId = user.Id });
            }

            admin.Roles.Add(new IdentityUserRole { RoleId = suRole.Id, UserId = admin.Id });

            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            result = context.SaveChangesAsync().Result;

            
            
            // Disabling seeding for production.
            return;




            // Seed Evaluations

            var xPhone = new Product { Name = "XPhone", Description = "This is the description for xPhone", OwnerId = admin.Id };
            var yPhone = new Product { Name = "YPhone", Description = "This is the description for yPhone", OwnerId = admin.Id };
            var zPhone = new Product { Name = "ZPhone", Description = "This is the description for zPhone", OwnerId = admin.Id };

            var products = new List<Product>
            {
                xPhone,
                yPhone,
                zPhone
            };

            products.ForEach(p => context.Products.Add(p));
            result = context.SaveChangesAsync().Result;

            // Seed Evaluations
            var evaluations = new List<Evaluation>
            {
                new Evaluation{Product = xPhone, Version = "1.0.0", Steps= 5, Owner = admin},
                new Evaluation{Product = yPhone, Version = "1.0.0", Steps= 3, Owner = admin},
                new Evaluation{Product = zPhone, Version = "1.0.0", Steps= 5, Owner = admin},
                new Evaluation{Product = xPhone, Version = "2.0.0", Steps= 3, Owner = admin},
                new Evaluation{Product = yPhone, Version = "1.1.0", Steps= 5, Owner = admin},
                new Evaluation{Product = zPhone, Version = "1.0.1", Steps= 3, Owner = admin}
            };

            evaluations.ForEach(r => context.Evaluations.Add(r));
            result = context.SaveChangesAsync().Result;

            // Seed attributes
            var names = new List<string> { "Robust Embodiment","Long Operational Time","Design",
                                           "Flashlight", "Belthook", "Radio Transmitter",
                                           "Bluetooth", "WiFi", "GPS", "HQ Camera"};

            var attributes = new List<Attribute>();
            System.Random random = new System.Random();

            foreach (var evaluation in evaluations)
            {
                for (int i = 0; i < 12; i++)
                {
                    attributes.Add(new Attribute { Name = names[random.Next(names.Count - 1)], Evaluation = evaluation, Order = i });

                    if (i > 3 && random.Next(100) < 50)
                    {
                        break;
                    }
                }
            }

            attributes.ForEach(s => context.Attributes.Add(s));
            result = context.SaveChangesAsync().Result;

            // Seed EvaluationUser
            var evaluationUsers = new List<EvaluationUser>();

            for (int i = 0; i < 100; i++)
            {
                var evaluationVersion = evaluations[random.Next(evaluations.Count - 1)].Version;
                var userFirstName = users[random.Next(users.Count - 1)].FirstName;
                Evaluation evaluation = context.Evaluations.First(r => r.Version == evaluationVersion);
                User user = context.Users.First(a => a.FirstName == userFirstName);

                evaluationUsers.Add(
                    new EvaluationUser
                    {
                        EvaluationId = evaluation.Id,
                        UserId = user.Id,
                        User = user,
                        Evaluation = evaluation,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    });
            };

            evaluationUsers = evaluationUsers.Distinct(new DistinctVersionUserComparer())
                             .ToList();
            evaluationUsers.ForEach(u => context.EvaluationUsers.Add(u));

            result = context.SaveChangesAsync().Result;

            // Seed Ratings
            var ratings = new List<Rating>();

            attributes = context.Attributes.ToList();

            for (int i = 0; i < 500; i++)
            {
                var attributesCount = attributes.Count;
                var attribute1 = attributes[random.Next(attributesCount - 1)];
                var attribute2 = attributes[random.Next(attributesCount - 1)];
                var evaluationUser = evaluationUsers[random.Next(evaluationUsers.Count - 1)];

                ratings.Add(
                    new Rating
                    {
                        EvaluationUserId = evaluationUser.Id,
                        AttributeId1 = attribute1.Id.Value,
                        AttributeId2 = attribute2.Id.Value,
                        Value = random.Next(1, 5)
                    });
            };

            ratings.ForEach(r => context.Ratings.Add(r));
            result = context.SaveChangesAsync().Result;

            base.Seed(context);
        }
    }

    class DistinctVersionUserComparer : IEqualityComparer<EvaluationUser>
    {

        public bool Equals(EvaluationUser x, EvaluationUser y)
        {
            return x.User.Email == y.User.Email;
        }

        public int GetHashCode(EvaluationUser obj)
        {
            return obj.EvaluationId.GetHashCode() ^
                   obj.User.Email.GetHashCode();
        }
    }
}
