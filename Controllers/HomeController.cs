using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TheWall.Models;

namespace TheWall.Models
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("")]
        public IActionResult Register(Wrapper Form)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == Form.User.Email))
                {
                    ModelState.AddModelError("User.Email", "Email already registered");
                    return Index();
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                Form.User.Password = Hasher.HashPassword(Form.User, Form.User.Password);
                _context.Add(Form.User);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("CurrentUser", Form.User.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return Index();
            }
        }

        [HttpPost("login")]
        public IActionResult Login(Wrapper Form)
        {
            if (ModelState.IsValid)
            {
                User ReturningUser = _context.Users.FirstOrDefault(u => u.Email == Form.LoginUser.LoginEmail);
                if (ReturningUser == null)
                {
                    ModelState.AddModelError("LoginUser.LoginPassword", "Invalid Email Address/Password");
                    return Index();
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(Form.LoginUser, ReturningUser.Password, Form.LoginUser.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginUser.LoginPassword", "Invalid Email Address/Password");
                    return Index();
                }
                HttpContext.Session.SetInt32("CurrentUser", ReturningUser.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return Index();
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            Wrapper Wrapper = new Wrapper();
            User ActiveUser = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("CurrentUser"));
            if (ActiveUser == null)
            {
                return RedirectToAction("Index");
            }
            Wrapper.User = ActiveUser;
            Wrapper.AllPosts = _context.Posts
                .Include(p => p.Creator)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Creator)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            // Wrapper.AllComments = _context.Comments.ToList();
            return View("Dashboard", Wrapper);
        }

        [HttpPost("post")]
        public IActionResult CreatePost(Wrapper Form)
        {
            User ActiveUser = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("CurrentUser"));
            if (ActiveUser == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                Form.Post.UserId = ActiveUser.UserId;
                _context.Add(Form.Post);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return Dashboard();
            }
        }

        [HttpPost("comment")]
        public IActionResult CreateComment(Wrapper Form)
        {
            User ActiveUser = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("CurrentUser"));
            if (ActiveUser == null)
            {
                return RedirectToAction("Index");
            }
            if (Form.Comment.UserId != ActiveUser.UserId)
            {
                return RedirectToAction("Dashboard");
            }
            if (ModelState.IsValid)
            {
                _context.Add(Form.Comment);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return Dashboard();
            }
        }

        [HttpPost("post/delete")]
        public IActionResult DeletePost(int postID)
        {
            User ActiveUser = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("CurrentUser"));
            if (ActiveUser == null)
            {
                return RedirectToAction("Index");
            }
            Post ToDelete = _context.Posts.FirstOrDefault(p => p.PostId == postID);
            if (ToDelete.UserId != ActiveUser.UserId)
            {
                return RedirectToAction("Dashboard");
            }
            _context.Remove(ToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("comment/delete")]
        public IActionResult DeleteComment(int commentID)
        {
            User ActiveUser = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("CurrentUser"));
            if (ActiveUser == null)
            {
                return RedirectToAction("Index");
            }
            Comment ToDelete = _context.Comments.FirstOrDefault(c => c.CommentId == commentID);
            if (ToDelete.UserId != ActiveUser.UserId)
            {
                return RedirectToAction("Dashboard");
            }
            _context.Remove(ToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}