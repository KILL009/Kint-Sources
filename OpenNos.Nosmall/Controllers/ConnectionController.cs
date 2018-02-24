using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using ShopApplicationByFizo.Models;

namespace ShopApplicationByFizo.Controllers
{
    public class ConnectionController : Controller
    {

        public ActionResult Index()
        {
            if(Session["auth"] != null && Session["user"] != null && Session["id"] != null)
            {
                return RedirectToAction("Index", "Product");
            }
            if(Session["id"] == null && Session["user"] != null && Session["auth"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            return View();
        }

        public ActionResult Login()
        {
            if (Session["auth"] != null && Session["user"] != null && Session["id"] != null)
            {
                return RedirectToAction("Index", "Product");
            }
            if(Session["id"] == null && Session["auth"] != null && Session["user"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account b)
        {
            if (Session["auth"] != null && Session["user"] != null && Session["id"] != null)
            {
                return RedirectToAction("Index", "Product");
            }
            if(Session["id"] == null && Session["auth"] != null && Session["user"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            if (ModelState.IsValid)
            {
                using (opennosEntities2 dc = new opennosEntities2())
                {
                    if(b.Password == null)
                    {
                        return RedirectToAction("Login", "Connection");
                    }
                    string pass = Sha512(b.Password);
                    var v = dc.Account.Where(a => a.Name.Equals(b.Name) && a.Password.Equals(pass)).FirstOrDefault();
                    if (v != null)
                    {
                        Session["auth"] = v.AccountId.ToString();
                        Session["user"] = v.Name.ToString();
                        return RedirectToAction("Index", "Personnage");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Connection");
                    }
                }
            }
            return View(b);
        }

        public string Sha512(string value)
        {
            SHA512 sha = SHA512.Create();
            byte[] data = sha.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public ActionResult Logout()
        {
            if (Session != null)
            {
                Session.Clear();
                return RedirectToAction("Login", "Connection");
            }
            else
            {
                return RedirectToAction("Login", "Connection");
            }
        }

    }
}