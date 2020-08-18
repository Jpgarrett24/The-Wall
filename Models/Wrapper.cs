using System.Collections.Generic;

namespace TheWall.Models
{
    public class Wrapper
    {
        public LoginUser LoginUser { get; set; }
        public List<User> AllUsers { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public List<Post> AllPosts { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> AllComments { get; set; }
    }
}