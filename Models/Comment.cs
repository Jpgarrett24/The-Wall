using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Post text is required")]
        [MinLength(2, ErrorMessage = "Post must be at least two characters long.")]
        [Display(Name = "Post Here: ")]
        public string Message { get; set; }

        [Display(AutoGenerateField = false)]
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; set; }
        public User Creator { get; set; }

        [Display(AutoGenerateField = false)]
        [HiddenInput(DisplayValue = false)]
        public int PostId { get; set; }
        public Post OriginalPost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}