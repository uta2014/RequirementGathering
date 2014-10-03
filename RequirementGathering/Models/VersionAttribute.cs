using System.Collections.Generic;

namespace RequirementGathering.Models
{
    public class VersionAttribute
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public int AttributeId { get; set; }

        public virtual Version Version { get; set; }
        public virtual Attribute Attribute { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
