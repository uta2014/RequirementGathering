using System.Collections.Generic;

namespace RequirementGathering.Models
{
    public class EvaluationAttribute
    {
        public int Id { get; set; }

        public int EvaluationId { get; set; }
        public virtual Evaluation Evaluation { get; set; }

        public int AttributeId { get; set; }
        public virtual Attribute Attribute { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
