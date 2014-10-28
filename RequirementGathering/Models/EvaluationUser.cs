
namespace RequirementGathering.Models
{
    public class EvaluationUser
    {
        public int Id { get; set; }

        public int EvaluationId { get; set; }
        public virtual Evaluation Evaluation { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public bool IsActive { get; set; }

        public EvaluationUser()
        {
            IsActive = true;
        }
    }
}
