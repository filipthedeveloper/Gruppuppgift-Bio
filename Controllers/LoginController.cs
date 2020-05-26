    
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
                //ANROPA säkerhetsgruppen för att skapa ny inloggning
            {
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                var response = client.PostAsJsonAsync("anvandares", p).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                    int inloggningsId = 5; //svaret från säkerhetsgruppen
                    Kund p = new Kund { Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn = efternamn, PersonNr = personNr, TelefonNr = telefonNr };
                    //länken till deras service?
                    p.InloggningsId = inloggningsId;
                    client.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                     response = client.PostAsJsonAsync("Kunder", p).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.Write("Success");
                    }
                    else
                    {
                        Console.Write("Error");
                    }
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