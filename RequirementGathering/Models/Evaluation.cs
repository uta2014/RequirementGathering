using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RequirementGathering.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public string Version { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public Evaluation()
        {
            IsActive = true;
        }

        #region Navigation Fields

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int EvaluationAttributeId { get; set; }
        public virtual ICollection<EvaluationAttribute> EvaluationAttributes { get; set; }

        #endregion

        public ICollection<Attribute> Attributes()
        {
            return EvaluationAttributes.Select(ea => ea.Attribute)
                                       .ToList();
        }
    }
}
