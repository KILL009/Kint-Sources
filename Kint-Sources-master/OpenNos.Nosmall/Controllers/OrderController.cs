using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using ShopApplicationByFizo.Models;

namespace ShopApplicationByFizo.Controllers
{
    public class OrderController : Controller
    {

        public ActionResult Index()
        {
            if(Session["auth"] == null ||  Session["user"] == null)
            {
                return RedirectToAction("Login", "Connection");
            }
            if(Session["id"] == null && Session["auth"] != null && Session["user"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            return View();
        }

        public ActionResult OrderA(int id, short vnum, byte amount)
        {
            if(Session["auth"] == null || Session["user"] == null)
            {
                return RedirectToAction("Login", "Connection");
            }
            if(Session["id"] == null && Session["user"] != null && Session["auth"] != null)
            {
                return RedirectToAction("Select", "Personnage");
            }
            using (opennosEntities2 context = new opennosEntities2())
            {
                IEnumerable<Character> result = context.Character;
                var productsQuery = result.Where(x => x.CharacterId == id);

                using (opennosEntities1 test = new opennosEntities1())
                {
                    IEnumerable<shopping> resultat = from Price in test.shopping
                                                     select Price;
                    var productQuery = resultat.Where(x => x.VNum == vnum);

                    if(productQuery == null)
                    {
                        Response.Redirect("Product/Index");
                    }

                    foreach (var prod in productsQuery)
                    {
                        foreach (var shop in productQuery)
                        {
                            if (shop.Price * Convert.ToByte(amount) > prod.NosheatDollar)
                            {
                                return View("Index");
                            }
                            else
                            {
                                int sky = prod.NosheatDollar - (shop.Price * Convert.ToByte(amount));
                                prod.NosheatDollar = sky;
                                if (MallServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]))
                                {
                                    MallServiceClient.Instance.SendItem(id, new MallItem()
                                    {
                                        ItemVNum = vnum,
                                        Amount = Convert.ToByte(amount),
                                        Rare = 0,
                                        Upgrade = 0
                                    });
                                    return View("OrderA");
                                }
                                else
                                {
                                    return View("Index");
                                }
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            return View("OrderA");
        }

    }
}