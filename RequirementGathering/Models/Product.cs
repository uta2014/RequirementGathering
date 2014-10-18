using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RequirementGathering.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Product()
        {
            IsActive = true;
        }

        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public ICollection<string> Versions()
        {
            return Evaluations.Select(v => v.Version)
                              .ToList();
        }
    }
}
