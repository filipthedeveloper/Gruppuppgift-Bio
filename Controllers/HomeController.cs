using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ja.Models;

namespace Ja.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            //Testkod för att kolla att sessionen fungerar
            //Sessionen fungerade för mig, då jag fick fram ur sessionen
            ////Hämta när man ska köpa biljetter
            Kund testKund = null;

            if (Session["Kund"] != null)
            {
                testKund = (Kund)Session["Kund"];
                Console.Write("test");
            }
            Console.Write("test");




            return View();
        }

        public ActionResult About()
        {
            return View();
        }

    }
}