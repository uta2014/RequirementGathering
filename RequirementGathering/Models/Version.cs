using System.Collections.Generic;
using System.Linq;

namespace RequirementGathering.Models
{
    public class Version
    {
        public int Id { get; set; }
        public int EvaluationId { get; set; }
        public string Number { get; set; }

        public virtual Evaluation Evaluation { get; set; }
        public virtual ICollection<VersionAttribute> VersionAttributes { get; set; }
        public IEnumerable<Attribute> Attributes
        {
            get
            {
                return VersionAttributes.Select(va => va.Attribute);
            }
        }
    }
}
