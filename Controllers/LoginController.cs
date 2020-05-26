    
using System;using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Ja.Models;
using System.Security;


namespace Ja.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Create(string email, string losenord, string fornamn, string efternamn, string personNr, string telefonNr)
        {
            using (var client = new HttpClient())
            {
                Anvandare p = new Anvandare { Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn = efternamn, PersonNr = personNr, TelefonNr = telefonNr };
                //länken till deras service?
                client.BaseAddress = new Uri("http://193.10.202.72/Kundservice/Kunder");
                var response = client.PostAsJsonAsync("Kundservice/Kunder", p).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                }
                else
                {
                    Console.Write("Error");
                }
            }



            return View();
        }
    }
}