using System;
using System.Collections.Generic;

namespace RequirementGathering.Models
{
    public class EvaluationUser
    {
        public int Id { get; set; }

        public int EvaluationId { get; set; }
        public virtual Evaluation Evaluation { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public string EvaluationLanguage { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public bool IsActive { get; set; }

        public EvaluationUser()
        {
            IsActive = true;
        }
    }
}
