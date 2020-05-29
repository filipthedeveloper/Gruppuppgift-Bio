using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Ja.Models;
using System.Security;
using Newtonsoft.Json;

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

            //CheckUser(email, losenord);
            using (var client = new HttpClient())
            {
                //CheckUser
                //Anvandare login = new Anvandare { Email = email, Losenord = losenord };
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                var response = client.PostAsJsonAsync("LoggaIn", login).Result;

                if (response.IsSuccessStatusCode) //Ändra
                {
                    string AnvSvar = response.Content.ReadAsStringAsync().Result;
                    Anvandare login2 = JsonConvert.DeserializeObject<Anvandare>(AnvSvar);

                    int inloggningsId = login2.Id;
                    Console.WriteLine("fungera pls");
                    ////skicka till newtonsoft???
                    //int inloggningsId = int.Parse(AnvSvar);
                    ////var AnvSvar = response.Content.ReadAsStringAsync().Result;
                    ////int inloggningsId = int.Parse(AnvSvar);


                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");

                        //???
                        //Försöker med Getanrop, men verkar inte fungera. Returnerar "44" + massor med andra knasiga saker
                        //var response2 = client2.PostAsJsonAsync("Kunder", inloggningsId).Result; //"Kunder" eller "GetKund"
                        var response2 = client2.GetAsync("Kunder/" + inloggningsId.ToString());



                        //string testResponse = response2.Content.ReadAsStringAsync().Result;

                        var testResponse = response2.Result.ToString();
                        Console.Write("???");
                        Kund loginKund = JsonConvert.DeserializeObject<Kund>(testResponse);
                        Console.Write("???");
                        //Spara i session efter login
                        //Session["Kund"] = loginKund;
                        //Console.Write("???");

                        //Hämta när man ska köpa biljetter
                        Kund testLogin = null;

                        
                        //Detta är inte färdigt ännu
                        if (Session["Kund"] != null)
                        {
                            testLogin = (Kund)Session["Kund"];
                            Console.Write("???");

                            //TempData["tempTest"] = Session["Kund"];
                        }



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
                var response = client.PostAsJsonAsync("SkapaAnvandare", NyAnvandare).Result;

                if (response.IsSuccessStatusCode)
                {
                    //response.IsSuccessStatusCode
                    //var inloggningsId = response;
                    string inloggningsId2 = response.Content.ReadAsStringAsync().Result;
                    //skicka till newtonsoft???
                    int inloggningsId =int.Parse(inloggningsId2);
                    Console.Write("Success");


                    Kund kund = new Kund { InloggningsId = inloggningsId, Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn=efternamn, PersonNr = personNr, TelefonNr =telefonNr,  Bonuspoang=0 };

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        var response2 = client2.PostAsJsonAsync("Kunder", kund).Result;
                        //läsa av responsen på samma sätt som vi fick en integer ovan

                        if (response2.IsSuccessStatusCode)
                        {
                            string testResponse = response2.Content.ReadAsStringAsync().Result;
                            Console.Write("Success");
                            kund = JsonConvert.DeserializeObject<Kund>(testResponse);

                            //Spara efter ny kund
                            Session["Kund"] = kund;
                            Console.Write("Success");

                            //Hämta när man ska köpa biljetter
                            Kund testKund = null;

                            if (Session["Kund"] != null)
                            {
                                testKund = (Kund)Session["Kund"];
                                Console.Write("Success");
                            }




                            //Session inloggningsId = InloggningsId
                            //Spara kundId?
                            //Man får tillbaka hela objektet
                            //Spara hela objektet i sessionen
                        }
                        else
                        {
                            Console.Write("Fail");
                        }
                    }
                    
                }
                else
                {
                    Console.Write("Error");
                }
              
            }

            return RedirectToAction("Index");
        }

        private bool CheckUser(string username, string password)
        {
            using (var client = new HttpClient())
            {
                Anvandare anvandareAttKolla = new Anvandare { Email = username, Losenord = password };

                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");

                var response = client.PostAsJsonAsync("LoggaIn", anvandareAttKolla).Result;

                if (response.IsSuccessStatusCode)
                {
                    //Webbservicen returnerar ett id om man kan logga in.
                    //Returnerar inte ett id, returnerar en hel sträng
                    
                    string id = response.Content.ReadAsStringAsync().Result;

                    if (id != "" && id != "0")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

        }

        //public ActionResult 
    }
}