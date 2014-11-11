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

            manager.Create(new IdentityRole { Name = "Administrator" });
            manager.Create(new IdentityRole { Name = "Researcher" });
            manager.Create(new IdentityRole { Name = "Evaluator" });

            // Seed Users
            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore);

            var adeel = new User { Email = "adeel@uta.fi", UserName = "adeel" };
            var cong = new User { Email = "cong@uta.fi", UserName = "cong" };
            var eija = new User { Email = "eija@uta.fi", UserName = "eija" };
            var ghassan = new User { Email = "ghassan@uta.fi", UserName = "ghassan" };
            var juho = new User { Email = "juho@uta.fi", UserName = "juho" };
            var liu = new User { Email = "liu@uta.fi", UserName = "Liu" };
            var teemu = new User { Email = "teemu@uta.fi", UserName = "teemu" };
            var toan = new User { Email = "toan@uta.fi", UserName = "toan" };

            var users = new List<User>
            { 
                adeel, cong, eija, ghassan, juho, liu, teemu, toan
            };

            foreach (var user in users)
            {
                userManager.Create(user, "DefaultPasscode!!");
                userManager.AddToRole(user.Id, "Researcher");
            }

            userManager.AddToRole(eija.Id, "Administrator");

            context.SaveChanges();

            // Seed Evaluations

            var xPhone = new Product { Name = "XPhone", Description = "This is the description for xPhone", Owner = eija };
            var yPhone = new Product { Name = "YPhone", Description = "This is the description for yPhone", Owner = eija };
            var zPhone = new Product { Name = "ZPhone", Description = "This is the description for zPhone", Owner = eija };

            var products = new List<Product>
            {
                xPhone,
                yPhone,
                zPhone
            };

            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            // Seed Evaluations
            var evaluations = new List<Evaluation>
            {
                new Evaluation{Product = xPhone, Version = "1.0.0", Owner = eija},
                new Evaluation{Product = yPhone, Version = "1.0.0", Owner = eija},
                new Evaluation{Product = zPhone, Version = "1.0.0", Owner = eija},
                new Evaluation{Product = xPhone, Version = "2.0.0", Owner = eija},
                new Evaluation{Product = yPhone, Version = "1.1.0", Owner = eija},
                new Evaluation{Product = zPhone, Version = "1.0.1", Owner = eija}
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

            // Seed VersionAttribute
            var evaluationAttributes = new List<EvaluationAttribute>();
            System.Random random = new System.Random();

            for (int i = 0; i < 100; i++)
            {
                var evaluationVersion = evaluations[random.Next(evaluations.Count)].Version;
                var attributeName = attributes[random.Next(attributes.Count)].Name;

                evaluationAttributes.Add(
                    new EvaluationAttribute
                    {
                        EvaluationId = context.Evaluations.First(r => r.Version == evaluationVersion).Id,
                        AttributeId = context.Attributes.First(a => a.Name == attributeName).Id
                    });
            };

            evaluationAttributes.Distinct(new DistinctVersionttributeComparer())
                             .ToList()
                             .ForEach(s => context.EvaluationAttributes.Add(s));

            context.SaveChanges();

            // Seed Ratings
            var ratings = new List<Rating>();

            evaluationAttributes = context.EvaluationAttributes.ToList();

            for (int i = 0; i < 500; i++)
            {
                var attributesCount = evaluationAttributes.Count;
                var evaluationAttribute1 = evaluationAttributes[random.Next(attributesCount)];
                var evaluationAttribute2 = evaluationAttributes[random.Next(attributesCount)];
                var user = users[random.Next(users.Count)];

                ratings.Add(
                    new Rating
                    {
                        UserId = user.Id,
                        EvaluationAttributeId1 = evaluationAttribute1.Id,
                        EvaluationAttributeId2 = evaluationAttribute2.Id,
                        Value1 = random.Next(1, 5),
                        Value2 = random.Next(1, 5)
                    });
            };

            ratings.ForEach(r => context.Ratings.Add(r));
            context.SaveChanges();
        }
    }

    class DistinctVersionttributeComparer : IEqualityComparer<EvaluationAttribute>
    {

        public bool Equals(EvaluationAttribute x, EvaluationAttribute y)
        {
            return x.AttributeId == y.AttributeId &&
                   x.EvaluationId == y.EvaluationId;
        }

        public int GetHashCode(EvaluationAttribute obj)
        {
            return obj.EvaluationId.GetHashCode() ^
                   obj.AttributeId.GetHashCode();
        }
    }
}
