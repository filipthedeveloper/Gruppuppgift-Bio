using System;
using System.Collections.Generic;
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
                //ANROPA säkerhetsgruppen för att skapa ny inloggning
                Anvandare a = new Anvandare { Email=email, Losenord = losenord };
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                var response = client.PostAsJsonAsync("SkapaAnvandare/{Email}/{Losenord}", a).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                    //Säkerhetsgruppen skall returnera ett Id och Behorighetsniva?
                    //Få med detta i Kund K?
                    //int referensId = 1; //svaret från säkerhetsgruppen
                    //string inloggningsId = response.Content.ToString();
                    //var inloggningsId = response.Content;
                    Kund k = new Kund { Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn=efternamn, TelefonNr=telefonNr, PersonNr=personNr, Bonuspoang=0 };

                    //k.InloggningsId = referensId;
                    client.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                     response = client.PostAsJsonAsync("Kunder", k).Result;
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