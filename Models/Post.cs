using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Post text is required")]
        [MinLength(2, ErrorMessage = "Post must be at least two characters long.")]
        [Display(Name = "Post A Message: ")]
        public string Message { get; set; }

        public int UserId { get; set; }
        public User Creator { get; set; }

        public List<Comment> Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}