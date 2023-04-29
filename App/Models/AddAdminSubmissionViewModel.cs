using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class AddAdminSubmissionViewModel
    {
        [Required(ErrorMessage = "Please Enter Event name..")]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        [Required]
        public DateTime OpenDate { get; set; }
        [Required]
        public DateTime ClosureDate { get; set; }
        [Required]
        public DateTime preClosureDate { get; set; }
        [Required]
        [Column(TypeName = "ntext")]
        public string? EventDecription { get; set; }
    }
}
