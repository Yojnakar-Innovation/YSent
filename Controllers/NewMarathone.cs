using Microsoft.AspNetCore.Mvc;
using YSent.Models;

namespace YSent.Controllers
{
    public class NewMarathone : Controller
    {

        public void manageUserAccess()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                Response.Redirect("/Home/Index");
            }
        }

        public IActionResult Index()
        {
            manageUserAccess();
            return View();
        }

       


           

    }
    }

