using System.ComponentModel.DataAnnotations;

namespace OnlineDrinkShop.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tag Name")]
        public string Tag_Name { get; set; } = string.Empty;
    }
}
