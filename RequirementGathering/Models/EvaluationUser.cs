using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementGathering.Models
{
    public class EvaluationUser
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Key, Column(Order = 1)]
        public int EvaluationId { get; set; }
        public virtual Evaluation Evaluation { get; set; }

        [Key, Column(Order = 2)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public bool IsActive { get; set; }

        public EvaluationUser()
        {
            IsActive = true;
        }
    }
}
