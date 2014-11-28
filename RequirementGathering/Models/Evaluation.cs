using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using RequirementGathering.Attributes;

namespace RequirementGathering.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        [Required]
        public string Version { get; set; }

        [Translatable]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public Evaluation()
        {
            IsActive = true;
        }

        #region Navigation Fields

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual ICollection<Attribute> Attributes { get; set; }
        public virtual ICollection<EvaluationUser> EvaluationUsers { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        #endregion

        public ICollection<User> Users()
        {
            return EvaluationUsers.Select(eu => eu.User)
                                  .ToList();
        }
    }
}
