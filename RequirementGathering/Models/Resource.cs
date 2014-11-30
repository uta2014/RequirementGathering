using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using RequirementGathering.Attributes;
using RequirementGathering.DAL;

namespace RequirementGathering.Models
{
    public class Resource
    {
        [Key, Column(Order = 0)]
        public string Culture { get; set; }

        [Key, Column(Order = 1)]
        public string Name { get; set; }

        public string Value { get; set; }

        #region Helpers
        // Probably using reflection not the best approach.
        public static string GetPropertyValue<T>(int id, string propertyName) where T : class
        {
            return GetPropertyValue<T>(id.ToString(), propertyName, Thread.CurrentThread.CurrentUICulture.Name);
        }

        public static string GetPropertyValue<T>(string id, string propertyName) where T : class
        {
            return GetPropertyValue<T>(id, propertyName, Thread.CurrentThread.CurrentUICulture.Name);
        }

        private static string GetPropertyValue<T>(string id, string propertyName, string culture) where T : class
        {
            Type entityType = typeof(T);
            string[] segments = propertyName.Split('.');

            if (segments.Length > 1)
            {
                entityType = Type.GetType("RequirementGathering.Models." + segments[0]);
                propertyName = segments[1];
            }

            if (entityType == null)
                return "?<invalid type>";

            var propertyInfo = entityType.GetProperty(propertyName);
            var translateableAttribute = propertyInfo.GetCustomAttributes(typeof(TranslatableAttribute), true)
                                        .FirstOrDefault();
            /*var requiredAttribute = propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true)
                                   .FirstOrDefault();*/

            if (translateableAttribute == null)
                return "?<this field has no translatable attribute>";

            var dbCtx = new RequirementGatheringDbContext();
            var className = entityType.Name;
            Resource resource = dbCtx.Resources.Where(r =>
                               (r.Culture == culture) &&
                                r.Name == className + id + propertyName).FirstOrDefault();

            if (resource != null)
                return resource.Value;

            //return requiredAttribute == null ? string.Empty : "?<translation not found>";
            return string.Empty;
        }
        #endregion
    }
}
