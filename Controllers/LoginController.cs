using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Ja.Models;
using System.Security;
using Newtonsoft.Json;
using System.ServiceModel;
using System.Diagnostics;



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
        public async Task<ActionResult> Index(string email, string losenord)
        {
            if (email == "" || losenord == "")
            {
                ModelState.AddModelError("", "Du måste fylla i både användarnamn och lösenord");
                return View();
            }

            //Kolla valid user, för att sedan tillåta genom Authorize
            bool validUser = false;
            validUser = CheckUser(email, losenord);

            if (!validUser)
            {
                ModelState.AddModelError("", "Inloggningen ej godkänd");
                return View();
            }

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
                    //Console.WriteLine("fungera pls");
                    ////skicka till newtonsoft???
                    //int inloggningsId = int.Parse(AnvSvar);
                    ////var AnvSvar = response.Content.ReadAsStringAsync().Result;
                    ////int inloggningsId = int.Parse(AnvSvar);


                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        List<Kund> KundInfo = new List<Kund>();

                        client2.DefaultRequestHeaders.Clear();
                        //Define request data format  
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response2 = await client2.GetAsync("Kunder");
                        //Console.Write("???");

                        if (response2.IsSuccessStatusCode)
                        {
                            //Sparar undan svarets innehåll från web api
                            var KundResponse = response2.Content.ReadAsStringAsync().Result;

                            //Deserializing på svaret från webapi, och sparar det i en lista
                            var results = JsonConvert.DeserializeObject<List<Kund>>(KundResponse);
                            KundInfo = results.Where(e => e.InloggningsId == inloggningsId).ToList();
                            //Console.Write("???");

                            Kund aktivKund = new Kund { 
                                InloggningsId = KundInfo[0].InloggningsId, 
                                Email = KundInfo[0].Email,
                                Losenord = KundInfo[0].Losenord,
                                Fornamn = KundInfo[0].Fornamn,
                                Efternamn = KundInfo[0].Efternamn,
                                TelefonNr = KundInfo[0].TelefonNr,
                                PersonNr = KundInfo[0].PersonNr,
                                Bonuspoang = KundInfo[0].Bonuspoang
                            };

                            //Console.Write("???");

                            if (validUser)
                            {
                                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(aktivKund.Email, false);

                                //Spara i session efter login om allt lyckas
                                Session["Kund"] = aktivKund;
                                //Console.Write("???");

                                if (Session["Kund"] != null)
                                {
                                    return RedirectToAction("Index", "Film");
                                }
                            }
                            //Lägger till errormeddelande
                            ModelState.AddModelError("", "Innloggningen ej godkänd");
                            return View();


                            
                        }
                        //Hämta när man ska köpa biljetter
                        //Detta är bara testkod
                        //Kund testLogin = null;

                        
                        ////Detta är inte färdigt ännu
                        //if (Session["Kund"] != null)
                        //{
                        //    testLogin = (Kund)Session["Kund"];
                        //    Console.Write("???");

                        //    //TempData["tempTest"] = Session["Kund"];
                        //}
                    }
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
                    string inloggningsId2 = response.Content.ReadAsStringAsync().Result;

                    int inloggningsId = int.Parse(inloggningsId2);
                    //Console.Write("Success");


                    Kund kund = new Kund { InloggningsId = inloggningsId, Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn=efternamn, PersonNr = personNr, TelefonNr =telefonNr,  Bonuspoang=0 };

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        var response2 = client2.PostAsJsonAsync("Kunder", kund).Result;
                        //läsa av responsen på samma sätt som vi fick en integer ovan

                        if (response2.IsSuccessStatusCode)
                        {
                            //Kod för att spara registreringen av användaren i en session?
                            //string testResponse = response2.Content.ReadAsStringAsync().Result;
                            //Console.Write("Success");
                            //kund = JsonConvert.DeserializeObject<Kund>(testResponse);

                            ////Spara efter ny kund
                            //Session["Kund"] = kund;
                            //Console.Write("Success");
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
                    //Webbservicen returnerar om man kan logga in.
                    //Returnerar inte ett id, returnerar en hel sträng
                    string id = response.Content.ReadAsStringAsync().Result;

                    if (id != "null")
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
    }
}