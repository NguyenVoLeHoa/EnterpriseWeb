using App.Entities;
using App.Web.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace App.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppUser class
public class User : IdentityUser
{
    [Required]
    public string? Firstname { get; set; }
    [Required]
    public string? Lastname { get; set; }
    public DateTime Dob { get; set; }
    public int Age { get; set; }
    [Required]
    public string? Genre { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; } = null!;

    [Display(Name = "Profile Picture")]
    public byte[]? ProfilePicture { get; set; }
    public virtual ICollection<UserActionLog> UserActionLog { get; set; }

}

