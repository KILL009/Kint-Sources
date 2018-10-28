using System.Linq;
using System.Web.Mvc;
using ShopApplicationByFizo.Models;

namespace ShopApplicationByFizo.Controllers
{
    public class ProductController : Controller
    {

        private opennosEntities1 oe = new opennosEntities1();

        public ActionResult Index()
        {
            if(Session["auth"] == null || Session["user"] == null)
            {
                return RedirectToAction("Login", "Connection");
            }
            if(Session["id"] == null && Session["auth"] != null && Session["user"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            ViewBag.Products = oe.shopping.ToList();
            return View();
        }

    }
}