using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ja.Models;
using Newtonsoft.Json;
using System.Web.Security;

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
            

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult Profil()
        {
            //Kund inloggadKund = null;
            //inloggadKund = (Kund)Session["Kund"];

            //var ktest = JsonConvert.DeserializeObject<List<Kund>>();


            return View(Session["KundLista"]);
        }

    }
}