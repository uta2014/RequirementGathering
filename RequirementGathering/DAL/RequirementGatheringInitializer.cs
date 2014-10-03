using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RequirementGathering.Models;

namespace RequirementGathering.DAL
{
    public class RequirementGatheringInitializer : DropCreateDatabaseIfModelChanges<RequirementGatheringDbContext>
    {
        protected override void Seed(RequirementGatheringDbContext context)
        {
            // Seed Roles
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);

            manager.Create(new IdentityRole { Name = "Super Administrator" });
            manager.Create(new IdentityRole { Name = "Administrator" });

            // Seed Users
            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore);

            var users = new List<User>
            { 
                new User{Email = "admin@example.com",UserName = "admin"},
                new User{Email = "uta@example.com",UserName = "uta"}
            };

            foreach (var user in users)
            {
                userManager.Create(user, "DefaultPasscode!!");
                userManager.AddToRole(user.Id, user.Email == "admin@exmplae.com" ? "Super Administrator" : "Administrator");
            }

            context.SaveChanges();

            // Seed Evaluations
            var evaluations = new List<Evaluation>
            {
                new Evaluation{Name = "XPhone"},
                new Evaluation{Name = "YPhone"},
                new Evaluation{Name = "ZPhone"}
            };

            evaluations.ForEach(r => context.Evaluations.Add(r));
            context.SaveChanges();

            // Seed attributes
            var attributes = new List<Attribute>
            {
                new Attribute{Name = "Robust Embodiment"},
                new Attribute{Name = "Long Operational Time"},
                new Attribute{Name = "Design"},
                new Attribute{Name = "Flashlight"},
                new Attribute{Name = "Belthook"},
                new Attribute{Name = "Radio Transmitter"},
                new Attribute{Name = "Bluetooth"},
                new Attribute{Name = "WiFi"},
                new Attribute{Name = "GPS"},
                new Attribute{Name = "HQ Camera"}
            };

            attributes.ForEach(s => context.Attributes.Add(s));
            context.SaveChanges();

            // Seed Versions
            var versions = new List<Version>
            {
                new Version{Number = "Robust Embodiment", Evaluation = evaluations[0]},
                new Version{Number = "Long Operational Time", Evaluation = evaluations[1]},
                new Version{Number = "Design", Evaluation = evaluations[2]},
                new Version{Number = "Flashlight", Evaluation = evaluations[0]},
                new Version{Number = "Belthook", Evaluation = evaluations[1]},
                new Version{Number = "Radio Transmitter", Evaluation = evaluations[2]},
                new Version{Number = "Bluetooth", Evaluation = evaluations[0]},
                new Version{Number = "WiFi", Evaluation = evaluations[1]},
                new Version{Number = "GPS", Evaluation = evaluations[2]},
                new Version{Number = "HQ Camera", Evaluation = evaluations[0]}
            };

            versions.ForEach(v => context.Versions.Add(v));
            context.SaveChanges();

            // Seed VersionAttribute
            var versionAttributes = new List<VersionAttribute>();
            System.Random random = new System.Random();

            for (int i = 0; i < 100; i++)
            {
                var versionNumber = versions[random.Next(versions.Count)].Number;
                var attributeName = attributes[random.Next(attributes.Count)].Name;

                versionAttributes.Add(
                    new VersionAttribute
                    {
                        VersionId = context.Versions.First(r => r.Number == versionNumber).Id,
                        AttributeId = context.Attributes.First(a => a.Name == attributeName).Id
                    });
            };

            versionAttributes.Distinct(new DistinctVersionttributeComparer())
                             .ToList()
                             .ForEach(s => context.VersionAttributes.Add(s));

            context.SaveChanges();

            // Seed Ratings
            var ratings = new List<Rating>();

            versionAttributes = context.VersionAttributes.ToList();

            for (int i = 0; i < 500; i++)
            {
                var versionAttribute = versionAttributes[random.Next(versionAttributes.Count)];
                var user = users[random.Next(users.Count)];

                ratings.Add(
                    new Rating
                    {
                        UserId = user.Id,
                        VersionAttributeId = versionAttribute.Id,
                        Value = random.Next(1, 5)
                    });
            };

            ratings.ForEach(r => context.Ratings.Add(r));
            context.SaveChanges();
        }
    }

    class DistinctVersionttributeComparer : IEqualityComparer<VersionAttribute>
    {

        public bool Equals(VersionAttribute x, VersionAttribute y)
        {
            return x.AttributeId == y.AttributeId &&
                x.VersionId == y.VersionId;
        }

        public int GetHashCode(VersionAttribute obj)
        {
            return obj.VersionId.GetHashCode() ^
                obj.AttributeId.GetHashCode();
        }
    }
}
