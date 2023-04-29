using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entities
{
    public class Department
    {
        //TODO: Add key
        //
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentID { get; set; }
        [Display(Name = "Department Name")]
        public string? DepartmentName { get; set; }
    }
}
