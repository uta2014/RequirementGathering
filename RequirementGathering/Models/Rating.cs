using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RequirementGathering.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int VersionAttributeId { get; set; }
        public string UserId { get; set; }
        [Range(1, 5)]
        [DefaultValue(1)]
        public int Value { get; set; }

        public virtual VersionAttribute VersionAttribute { get; set; }
        public virtual User User { get; set; }
    }
}
