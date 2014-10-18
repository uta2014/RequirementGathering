using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RequirementGathering.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        [DefaultValue(1)]
        public int Value { get; set; }

        #region Navigation Fields

        public int EvaluationAttributeId { get; set; }
        public virtual EvaluationAttribute EvaluationAttribute { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        #endregion

        public Evaluation Evaluation
        {
            get { return EvaluationAttribute.Evaluation; }
        }

        public Attribute Attribute()
        {
            return EvaluationAttribute.Attribute;
        }
    }
}
