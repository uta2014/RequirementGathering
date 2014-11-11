using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RequirementGathering.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        public string Version { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public Evaluation()
        {
            IsActive = true;
        }

        #region Navigation Fields

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual ICollection<EvaluationAttribute> EvaluationAttributes { get; set; }
        public virtual ICollection<EvaluationUser> EvaluationUsers { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        #endregion

        public ICollection<Attribute> Attributes()
        {
            return EvaluationAttributes.Select(ea => ea.Attribute)
                                       .ToList();
        }

        public ICollection<User> Users()
        {
            return EvaluationUsers.Select(eu => eu.User)
                                  .ToList();
        }
    }
}
