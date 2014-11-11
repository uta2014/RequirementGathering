using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using RequirementGathering.Attributes;
using RequirementGathering.Helpers;
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
        public DbSet<Models.Attribute> Attributes { get; set; }
        public DbSet<EvaluationAttribute> EvaluationAttributes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Resource> Resources { get; set; }

        public override Task<int> SaveChangesAsync()
        {
            ObjectContext ctx = ((IObjectContextAdapter)this).ObjectContext;

            List<ObjectStateEntry> objectDeletedStateEntryList =
                ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted)
               .ToList();

            List<ObjectStateEntry> objectCreateOrModifiedStateEntryList =
                ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added
                                                           | EntityState.Modified)
               .ToList();

            // First handle the delition case,
            // before making changes to entry state
            bool changed = UpdateResources(objectDeletedStateEntryList);

            // Now save the changes
            int result = base.SaveChangesAsync().Result;

            // Finally handle the remaining cases
            changed |= UpdateResources(objectCreateOrModifiedStateEntryList);

            if (changed)
                return base.SaveChangesAsync();

            return Task.FromResult<int>(result);
        }

        private bool UpdateResources(List<ObjectStateEntry> objectStateEntryList)
        {
            bool changed = false;

            foreach (ObjectStateEntry entry in objectStateEntryList)
            {
                var typeName = entry.EntitySet.ElementType.Name;

                if (entry.IsRelationship || typeName == "Resource")
                    return false;

                var type = Type.GetType("RequirementGathering.Models." + typeName);

                if (type == null) // When seeds run (db created for the first-time), sometimes types might not be create
                    return false;

                if (entry.State == EntityState.Deleted)
                {
                    changed |= DeleteResources(type, typeName, entry);
                    continue;
                }

                foreach (var propertyInfo in type.GetProperties())
                {
                    var attribute = propertyInfo.GetCustomAttributes(typeof(TranslatableAttribute), true).FirstOrDefault();

                    if (attribute == null)
                        continue;

                    CurrentValueRecord current = entry.CurrentValues;
                    object idField = current.GetValue(current.GetOrdinal("Id"));

                    if (idField == null)
                        continue;

                    var id = idField.ToString();
                    var propertyName = propertyInfo.Name;
                    string newValue = current.GetValue(current.GetOrdinal(propertyName)).ToString();
                    var name = typeName + id + propertyName;

                    Resource existingResource = this.Resources.Find(Thread.CurrentThread.CurrentUICulture.Name, name);

                    if (existingResource == null)
                    {
                        foreach (var culture in CultureHelper.Cultures)
                        {
                            this.Resources.Add(new Resource
                            {
                                Culture = culture,
                                Name = name,
                                Value = newValue
                            });

                            changed |= true;
                        }
                    }
                    else
                    {
                        existingResource.Value = newValue;
                        changed |= true;
                    }
                }
            }

            return changed;
        }

        private bool DeleteResources(Type type, string typeName, ObjectStateEntry entry)
        {
            bool changed = false;
            var firstKey = entry.EntityKey.EntityKeyValues.Where(k => k.Key.Equals("Id", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (firstKey == null)
                return false;

            var id = firstKey.Value.ToString();

            foreach (var propertyInfo in type.GetProperties())
            {
                var name = typeName + id + propertyInfo.Name;

                foreach (var culture in CultureHelper.Cultures)
                {
                    Resource existingResource = this.Resources.Find(culture, name);

                    if (existingResource == null)
                        continue;

                    this.Resources.Remove(existingResource);
                    changed |= true;
                }
            }

            return changed;
        }

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
