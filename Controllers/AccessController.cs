 using Microsoft.AspNetCore.Mvc;
using QLBN.Models;
using QLBN.Models.Authentication;


namespace QLBN.Controllers
{
    
    public class AccessController : Controller
    {
        QLBNContext db = new QLBNContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return View();
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                var u = db.Users.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("Username", u.Username.ToString()); // thiết lập một session có tên username được gán bởi tên người dùng, xảy ra khi nhập dữ liệu cho người dùng có username nằm trong cơ sở dữ liệu và cần đảo bảo nó chwua tồn tại ==null

                    if (user.Username == "admin")
                    {

                        return View("~/Areas/Admin/Views/HomeAdmin/Index.cshtml");
                    }

                    else return RedirectToAction("Index", "Home");
                }
            }
            return View();



        }
        [HttpGet]
        public IActionResult SignUp() 
        {

            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User newuser)
        {
            
            if(ModelState.IsValid)
            {
                var user = new User
                {
                    Username = newuser.Username,
                    Password = newuser.Password,
                    Role = "user"

                };
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else { return View(); } 
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login", "Access");
        }
    }
}
