using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RequirementGathering.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public Evaluation()
        {
            IsActive = true;
        }

        public virtual ICollection<Version> Versions { get; set; }
        public ICollection<Attribute> Attributes()
        {
            return Versions.SelectMany(v => v.VersionAttributes
                                             .Select(va => va.Attribute))
                           .ToList();
        }
    }
}
