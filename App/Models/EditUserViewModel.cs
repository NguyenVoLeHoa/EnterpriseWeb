using App.Areas.Identity.Data;
using App.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.Models
{
    public class EditUserViewModel
    {
        public User User { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
