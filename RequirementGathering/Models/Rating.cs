using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(0, 5, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRangeMinMax")]
        [DefaultValue(0)]
        public int Value { get; set; }

        #region Navigation Fields

        public int AttributeId1 { get; set; }
        [ForeignKey("AttributeId1"), InverseProperty("Ratings")]
        public virtual Attribute Attribute1 { get; set; }

        public int AttributeId2 { get; set; }
        [ForeignKey("AttributeId2")]
        public virtual Attribute Attribute2 { get; set; }

        public int EvaluationUserId { get; set; }
        public virtual EvaluationUser EvaluationUser { get; set; }

        #endregion
    }
}
