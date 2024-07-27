using System.ComponentModel.DataAnnotations;

namespace OnlineDrinkShop.Areas.Admin.Models
{
    public class UserRoleVm
    {
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; } = string.Empty;
    }
}
