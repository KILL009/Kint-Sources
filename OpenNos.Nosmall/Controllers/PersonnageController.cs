using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopApplicationByFizo.Models;

namespace ShopApplicationByFizo.Controllers
{
    public class PersonnageController : Controller
    {
        
        public ActionResult Index()
        {
            if(Session["auth"] == null)
            {
                return RedirectToAction("Login","Connection");
            }
            if(Session["id"] != null && Session["auth"] != null)
            {
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public ActionResult Select(string name)
        {
            if (Session["auth"] == null)
            {
                return RedirectToAction("Login", "Connection");
            }
            if (Session["id"] != null && Session["auth"] != null)
            {
                return RedirectToAction("Index", "Product");
            }
            using (opennosEntities2 context = new opennosEntities2())
            {
                IEnumerable<Character> result = context.Character;
                var product = result.Where(x => x.Name == name);
                foreach (var chara in product)
                {
                    Session["id"] = chara.CharacterId;
                }
            }
            return RedirectToAction("Index", "Product");
        }

    }
}