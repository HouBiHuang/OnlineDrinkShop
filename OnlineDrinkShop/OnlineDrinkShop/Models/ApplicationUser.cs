using Microsoft.AspNetCore.Identity;

namespace OnlineDrinkShop.Models
{
    public class ApplicationUser: IdentityUser
    {
        public int BonusPoints { get; set; } = 0;
        public string? UserFullName { get; set; }
    }
}