using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequirementGathering.Attributes;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Models
{
    public class Attribute
    {
        public int? Id { get; set; }

        [Display(Name = "NameDisplay", ResourceType = typeof(Resources))]
        [Translatable]
        public string Name { get; set; }

        #region Navigation Fields

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        public int EvaluationId { get; set; }
        public virtual Evaluation Evaluation { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }

        #endregion
    }
}
