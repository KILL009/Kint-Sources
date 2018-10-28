using System.Collections.Generic;
using System.Web.Mvc;
using ShopApplicationByFizo.Models;

namespace ShopApplicationByFizo.Controllers
{
    public class ShoppingCartController : Controller
    {

        private opennosEntities1 oe = new opennosEntities1();

        public ActionResult Index()
        {
            if(Session["user"] == null || Session["auth"] == null)
            {
                return RedirectToAction("Login", "Connection");
            }
            if(Session["id"] == null && Session["user"] != null && Session["auth"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            return View();
        }

        public ActionResult OrderNow(int vnum)
        {
            if(Session["auth"] == null || Session["user"] == null)
            {
                return RedirectToAction("Login", "Connection");
            }
            if(Session["id"] == null && Session["user"] != null && Session["auth"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            if(Session["id"] != null)
            {
                List<Item> cart = new List<Item>
                {
                    new Item(oe.shopping.Find(vnum), 1)
                };
                Session["cart"] = cart;
            }
            else
            {

            }
            return View("cart");
        }

    }
}