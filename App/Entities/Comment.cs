using App.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entities
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }
        public string? Content { get; set; }
        public DateTime ContentDate { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public virtual Post? Post { get; set; }
    }
}
