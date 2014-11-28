using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequirementGathering.Attributes;

namespace RequirementGathering.Models
{
    public class Attribute
    {
        public int Id { get; set; }

        [Translatable]
        public string Name { get; set; }

        #region Navigation Fields

        [Required]
        public int EvaluationId { get; set; }
        public virtual Evaluation Evaluation { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }

        #endregion
    }
}
