using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRangeMinMax")]
        [DefaultValue(1)]
        public int Value1 { get; set; }

        [Range(1, 5, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRangeMinMax")]
        [DefaultValue(1)]
        public int Value2 { get; set; }

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
