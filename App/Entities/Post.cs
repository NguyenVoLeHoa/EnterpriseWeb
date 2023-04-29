using App.Web.Entities;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace App.Entities
{
    public class Post
    {
        public int PostId { get; set; }

        public int EventId { get; set; }

        public Event? Event { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Brief { get; set; }

        public int View { get; set; } = 0;

        public int Like { get; set; } = 0;

        public int Dislike { get; set; } = 0;

        public virtual ICollection<UserActionLog>? UserActionLog { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
