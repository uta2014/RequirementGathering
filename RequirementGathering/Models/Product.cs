using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public bool IsActive { get; set; }

        public Product()
        {
            IsActive = true;
        }

        #region Navigation Fields

        public virtual ICollection<Evaluation> Evaluations { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        #endregion

        public ICollection<string> Versions()
        {
            return Evaluations.Select(v => v.Version)
                              .ToList();
        }
    }
}
