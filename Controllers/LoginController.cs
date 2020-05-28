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

        [HttpPost]
        public ActionResult Index(string email, string losenord)
        {
            Anvandare login = new Anvandare();
            login.Email = email;
            login.Losenord = losenord;
            //Anropa säkerhetsgruppen med email och lösenord
            //Vad vill de ha av oss för login? Objekt av vilken typ? Objekt som innehåller email och losenord
            //Jag gissar på att grupp 4 vill ha av objekt Anvandare, så börjar programmera in det
            using (var client = new HttpClient())
            {
                //Anvandare login = new Anvandare { Email = email, Losenord = losenord };
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                var response = client.PostAsJsonAsync("LoggaIn", login).Result; //De returnerar ett objekt som heter Anv? Skall vi skapa en ny modell? Anv?

                if (response.IsSuccessStatusCode) //Ändra
                {
                    //response != 0
                    var AnvSvar = response.Content.ReadAsStringAsync().Result;
                    int InloggningsId = Int32.Parse(AnvSvar);
                    //var InloggningsId = response;

                    //Anvandare testObj = new Anvandare();
                    //testObj.
                    //var testhej = AnvSvar;
                    Console.Write("Success");
                    //var testId = 1;

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        var response2 = client.PostAsJsonAsync("GetKund", InloggningsId).Result; //Här tar det stopp
                        /*StatusCode: 404, ReasonPhrase: 'Not Found', Version: 1.1*/
                        var test = response2.Content;

                        //Session["potatis"] = test;

                        //Console.Write("Success");
                    }
                    //client.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                    //var response2 = client.PostAsJsonAsync("GetKund", testId).Result;


                }
                else
                {
                    Console.Write("Error");
                }
            }
            


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
                //ANROPA säkerhetsgruppen för att skapa nytt konto
                Anvandare NyAnvandare = new Anvandare { Email=email, Losenord = losenord };
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                var response = client.PostAsJsonAsync("SkapaAnvandare", NyAnvandare).Id;

                if (response != 0) //Kan behöva att ändras
                {
                    //response.IsSuccessStatusCode
                    var inloggningsId = response;
                    Console.Write("Success");

                    //string inloggningsId = response.Content.ToString();
                    //SkapaAnvandare/{Email}/{Losenord}


                    //var inloggningsId = response.Content;
                    //PostKund(Kund kund); är en metod som vi skall skicka till. Detta skapar en ny kund hos grupp 2
                    //Kund skall innehålla: InloggningsId, Email, Losenord, Fornamn, Efternamn, PersonNr, TelefonNr, Bonuspoang
                    Kund kund = new Kund { InloggningsId = inloggningsId, Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn=efternamn, PersonNr = personNr, TelefonNr =telefonNr,  Bonuspoang=0 };

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        var response2 = client2.PostAsJsonAsync("PostKund", kund).Result; /*StatusCode: 404, ReasonPhrase: 'Not Found', Version: 1.1*/
                        if (response2.IsSuccessStatusCode) //Responsen blir false
                        {
                            Console.Write("Success");
                        }
                        else
                        {
                            Console.Write("Fail");
                        }
                    }
                    
                    

                    //Console.Write("Success");
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    Console.Write("Success");
                    //}
                    //else
                    //{
                    //    Console.Write("Error");
                    //}
                }
                else
                {
                    Console.Write("Error");
                }
              
            }



            return RedirectToAction("Index");
        }

        //public ActionResult 
    }
}