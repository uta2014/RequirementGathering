using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RequirementGathering.Models
{
    public class Attribute
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public Attribute()
        {
            IsActive = true;
        }

        #region Navigation Fields
        public int EvaluationAttributeId { get; set; }
        public virtual ICollection<EvaluationAttribute> EvaluationAttributes { get; set; }
        #endregion

        public ICollection<Evaluation> Evaluation()
        {
            return EvaluationAttributes.Select(ea => ea.Evaluation)
                                       .ToList();
        }
    }
}
