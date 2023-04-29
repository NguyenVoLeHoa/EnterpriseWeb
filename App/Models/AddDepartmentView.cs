using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class AddDepartmentView
    {
        [Required(ErrorMessage = "Please Enter Department name..")]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
    }
}
