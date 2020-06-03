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
using System.Web.Security;



namespace Ja.Controllers
{
    public class LoginController : Controller
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string email, string losenord)
        {
            //Om båda fält är tomma
            if (email == "" || losenord == "")
            {
                ModelState.AddModelError("", "Du måste fylla i både användarnamn och lösenord");
                Logger.Error("Båda fält måste fyllas i.");
                return View();
            }

            //Kolla valid user, för att sedan tillåta genom Authorize
            bool validUser = false;
            validUser = CheckUser(email, losenord);

            //Om inloggningen ej var godkänd
            if (!validUser)
            {
                ModelState.AddModelError("", "Inloggningen ej godkänd");
                Logger.Error("Felaktig inloggning");
                return View();
            }
            
            //Skapar användare
            Anvandare login = new Anvandare();
            login.Email = email;
            login.Losenord = losenord;
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                //Post för att logga in
                var response = client.PostAsJsonAsync("LoggaIn", login).Result;

                if (response.IsSuccessStatusCode)
                {
                    //Sparar svaret
                    string AnvSvar = response.Content.ReadAsStringAsync().Result;
                    Anvandare login2 = JsonConvert.DeserializeObject<Anvandare>(AnvSvar);

                    int inloggningsId = login2.Id;


                    using (var client2 = new HttpClient())
                    {
                        //Ny adress
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        List<Kund> KundInfo = new List<Kund>();

                        client2.DefaultRequestHeaders.Clear();
                        //Definerar dataformat
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //Get kunder
                        HttpResponseMessage response2 = await client2.GetAsync("Kunder");

                        if (response2.IsSuccessStatusCode)
                        {
                            //Sparar undan svarets innehåll från web api
                            var KundResponse = response2.Content.ReadAsStringAsync().Result;

                            //Deserializing på svaret från webapi, och sparar det i en lista
                            var results = JsonConvert.DeserializeObject<List<Kund>>(KundResponse);
                            KundInfo = results.Where(e => e.InloggningsId == inloggningsId).ToList();

                            //Sparar den aktiva kunden
                            Kund aktivKund = new Kund
                            {
                                InloggningsId = KundInfo[0].InloggningsId,
                                Email = KundInfo[0].Email,
                                Losenord = KundInfo[0].Losenord,
                                Fornamn = KundInfo[0].Fornamn,
                                Efternamn = KundInfo[0].Efternamn,
                                TelefonNr = KundInfo[0].TelefonNr,
                                PersonNr = KundInfo[0].PersonNr,
                                Bonuspoang = KundInfo[0].Bonuspoang
                            };

                            //Om inloggningen lyckas
                            if (validUser)
                            {
                                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(aktivKund.Email, false);

                                //Spara i session efter login om allt lyckas
                                Session["KundSession"] = aktivKund;
                                Session["KundLista"] = KundInfo;

                                if (Session["KundSession"] != null)
                                {
                                    //Tempdata för en alert notis till användaren
                                    TempData["login"] = "Inloggningen lyckades!";
                                    return View();
                                }
                            }
                            //Lägger till errormeddelande om den kommer hit
                            ModelState.AddModelError("", "Innloggningen ej godkänd.");
                            Logger.Error("Ej godkänd inloggning.");
                            return View();


                            
                        }
                        else
                        {
                            Logger.Error("Response2 fail i inloggning, kunde ej hämta kunder");
                        }
                    }
                }
                else
                {
                    Logger.Error("Response fail vid inloggning, kunde ej hämta användare.");
                    //Console.Write("Error");
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
                //Anropa säkerhetsgruppen för att skapa nytt konto med post
                Anvandare NyAnvandare = new Anvandare { Email=email, Losenord = losenord };
                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                var response = client.PostAsJsonAsync("SkapaAnvandare", NyAnvandare).Result;

                if (response.IsSuccessStatusCode)
                {
                    //Sparar id
                    string inloggningsId2 = response.Content.ReadAsStringAsync().Result;
                    //Parse id från sträng till int
                    int inloggningsId = int.Parse(inloggningsId2);

                    //Ny kund
                    Kund kund = new Kund { InloggningsId = inloggningsId, Email = email, Losenord = losenord, Fornamn = fornamn, Efternamn=efternamn, PersonNr = personNr, TelefonNr =telefonNr,  Bonuspoang=0 };

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("http://193.10.202.72/Kundservice/");
                        //Post, skickar nya kunden
                        var response2 = client2.PostAsJsonAsync("Kunder", kund).Result;

                        //Lista kund
                        List<Kund> KundInfo = new List<Kund>();

                        if (response2.IsSuccessStatusCode)
                        {
                            //Sparar svar
                            var KundResponse = response2.Content.ReadAsStringAsync().Result;
                            //Deserialize objekt
                            var results = JsonConvert.DeserializeObject<Kund>(KundResponse);

                            //Kolla valid user, för att sedan tillåta genom Authorize
                            bool validUser = false;
                            validUser = CheckUser(email, losenord);

                            if (validUser)
                            {
                                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(kund.Email, false);

                                //Text till användares notis
                                TempData["create"] = "Ditt konto har skapats. Det går nu att logga in.";
                                
                                return RedirectToAction("Index","Login");
                            }
                            else
                            {
                                Logger.Error("Validuser false, response2 i create");
                            }
                        }
                        else
                        {
                            Logger.Error("Fail på response av kundservice kunder i create, response2.");
                        }
                    }
                    
                }
                else
                {
                    Logger.Error("Response fel på skapa konto.");
                }

            }
            return RedirectToAction("Index", "Login");
        }

        private bool CheckUser(string username, string password)
        {
            using (var client = new HttpClient())
            {
                //Användare objekt för att kolla mot tjänsternas databaser
                Anvandare anvandareAttKolla = new Anvandare { Email = username, Losenord = password };

                client.BaseAddress = new Uri("http://193.10.202.74/inlogg/");
                //Sparar response
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
                        Logger.Error("id i checkuser är null.");
                        return false;
                    }
                }
                else
                {
                    Logger.Error("Kolla användare i checkuser är false, finns inte.");
                    return false;
                }
            }

        }

        //Loggar ut användaren
        public ActionResult LogOut()
        {
            //Funktinoer för att logga ut användaren och avsluta session
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("LogOut2", "login");
        }

        //Loggar ut användaren
        public ActionResult LogOut2()
        {
            //Text som låter användaren få veta att man blivit utloggad
            TempData["logout"] = "Du är nu utloggad.";
            return RedirectToAction("Index", "Home");
        }
    }
}