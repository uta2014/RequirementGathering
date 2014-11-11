using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using RequirementGathering.Models;

namespace RequirementGathering.DAL
{
    public class RequirementGatheringDbContext : IdentityDbContext<User>
    {
        public RequirementGatheringDbContext()
            : base("RequirementGatheringDbContext")
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<EvaluationUser> EvaluationUsers { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<EvaluationAttribute> EvaluationAttributes { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Rating>()
                .HasRequired(c => c.EvaluationAttribute1)
                .WithMany()
                .WillCascadeOnDelete(false);
        }

        public static RequirementGatheringDbContext Create()
        {
            return new RequirementGatheringDbContext();
        }
    }
}
