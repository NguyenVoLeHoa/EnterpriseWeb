using App.Areas.Identity.Data;
using App.Entities;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Web.Entities
{
    public class UserActionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionLogId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }


        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool Liked { get; set; }

        public DateTime UpdatedDate { get; set; }


    }

    public enum LikeType
    {
        Like = 0,
        Dislike = 1
    }
}
