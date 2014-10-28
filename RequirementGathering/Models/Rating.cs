using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementGathering.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        [DefaultValue(1)]
        public int Value1 { get; set; }

        [Range(1, 5)]
        [DefaultValue(1)]
        public int Value2 { get; set; }

        #region Navigation Fields

        public int EvaluationAttributeId1 { get; set; }
        [ForeignKey("EvaluationAttributeId1"), InverseProperty("Ratings")]
        public virtual EvaluationAttribute EvaluationAttribute1 { get; set; }

        public int EvaluationAttributeId2 { get; set; }
        [ForeignKey("EvaluationAttributeId2")]
        public virtual EvaluationAttribute EvaluationAttribute2 { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        #endregion

        public Evaluation Evaluation
        {
            get { return EvaluationAttribute1.Evaluation; }
        }

        public Attribute Attribute1
        {
            get { return EvaluationAttribute1.Attribute; }
        }

        public Attribute Attribute2
        {
            get { return EvaluationAttribute2.Attribute; }
        }
    }
}
