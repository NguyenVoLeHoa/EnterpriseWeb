using App.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class EventModel
    {
        //[Required]
        //[System.ComponentModel.DataAnnotations.Key]
        public int EventID { get; set; }

        [Required]
        [StringLength(100)]
        public string? EventName { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string? EventDecription { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }
        [Required]
        public DateTime preClosureDate { get; set; }
        [Required]
        public DateTime ClosureDate { get; set; }

        public List<Post>? Posts { get; set; }

        public double HoursRemain { get; set; }
    }
}
