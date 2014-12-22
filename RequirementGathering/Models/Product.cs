using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using RequirementGathering.Attributes;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "NameDisplay", ResourceType = typeof(Resources))]
        [Translatable]
        public string Name { get; set; }

        [Translatable]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual string CulturedName { get { return Resource.GetPropertyValue<Product>(Id, "Name"); } }

        public bool IsActive { get; set; }

        public Product()
        {
            IsActive = false;
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
