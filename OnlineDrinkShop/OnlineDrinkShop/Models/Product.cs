using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineDrinkShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        public string ProductName { get; set; } = string.Empty;

        public string? Image { get; set; }

        [Required]
        public int SmallPrice { get; set; }

        [Required]
        public int BigPrice { get; set; }

        [Required]
        [Display(Name = "Tag")]
        public int TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag? Tag { get; set; }

        public string? Remark { get; set; }

        [Required]
        public bool SugarLevelIsAvailable { get; set; }

        [Required]
        public bool IceLevelIsAvailable { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
    }
}
