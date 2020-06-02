using Ja.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ServiceModel;
using System.Diagnostics;
using System.Linq;

namespace Ja.Controllers
{
    public class BiljettController : Controller
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //URL länk
        string Baseurl = "http://193.10.202.72/Biljettservice/";
        [Authorize]
        public async Task<ActionResult> Boka(int id)
        {
            //Lista för platserna
            List<BokadePlatser> BokadePlatser = new List<BokadePlatser>();
            try
            {
                using (var client = new HttpClient())
                {
                    //Hämtar länken
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    //Definerar dataformat 
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //GET på bokade platser
                    HttpResponseMessage Res = await client.GetAsync("Bokadeplatser");

                    //Kollar responsens status 
                    if (Res.IsSuccessStatusCode)
                    {
                        //Sparar undan svaret  
                        var BokadePlatserSvar = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing på svaret till en lista
                        BokadePlatser = JsonConvert.DeserializeObject<List<BokadePlatser>>(BokadePlatserSvar);
                        //Söker ut alla bokade platser där visninschema stämmer med id, får en bokning
                        BokadePlatser = BokadePlatser.Where(e => e.VisningsSchemaId == id).ToList();
                    }
                }
            }
            catch (Exception)
            {
                Logger.Error("Error, lyckades ej hämta bokningar");
                return RedirectToAction("Index", "Biljett");
                
            }
            //Sparar undan id i tempdata, för att senare skicka detta för bokningarna
            TempData["tempSchemaId"] = id;
            //Skickar bokningen till sidan
            return View(BokadePlatser);
        }

        //string BaseurlSchema = "http://193.10.202.71/Filmservice/film";
        //[Authorize]
        public async Task<ActionResult> VisningsSchema(string titel)
        {
            //Lista för schema
            List<VisningsSchema> SchemaLista = new List<VisningsSchema>();
            try
            {
                using (var client = new HttpClient())
                {
                    //Hämtar url 
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    //Definerar dataformat 
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //GET, hämtar schemat
                    HttpResponseMessage Res = await client.GetAsync("VisningsSchema");

                    //Kollar om get lyckades
                    if (Res.IsSuccessStatusCode)
                    {
                        //Sparar svaret
                        var SchemaResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing på svaret till en lista
                        SchemaLista = JsonConvert.DeserializeObject<List<VisningsSchema>>(SchemaResponse);
                        //Letar i listan efter specifika saker i schemat, titeln
                        SchemaLista = SchemaLista.Where(e => e.FilmTitel == titel).ToList();

                    }
                }
            }
            catch (Exception)
            {
                Logger.Error("Error, kunde inte visa upp visningsschema");
                return RedirectToAction("Index", "Film");

            }
            //rVy med schema
            return View(SchemaLista);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Boka(string antalIn)
        {
            //Parse på bokning, från sträng till int
            try
            {
                int antal = int.Parse(antalIn);

            //Hämta när man ska köpa biljetter
            Kund inloggadKund = null;

            //Sparar i session
            if (Session["KundSession"] != null)
            {
                inloggadKund = (Kund)Session["KundSession"];
            }
            else
            {
                Logger.Error("Error med session");
                return View();
            }

            //Parse på schema id
            int schema = int.Parse(TempData["tempSchemaId"].ToString());

            //Visningsid och antal bokade platser
            BokadePlatser nyPlatser = new BokadePlatser { AntalBokadePlatser = antal, VisningsSchemaId = schema };
            Bokningar nyBokning = new Bokningar { VisningsSchemaId = schema, KundId = inloggadKund.InloggningsId };

            try
            {
                using (var clientBoka = new HttpClient())
                {
                    clientBoka.BaseAddress = new Uri("http://193.10.202.72/BiljettService/");
                    //Loop, gör bokning enligt antal önskade platser
                    for (int i = 0; i < antal; i++)
                    {
                        var responseBoka = clientBoka.PostAsJsonAsync("Bokningar", nyBokning).Result;
                    }
                    //Efter loopen, skicka till Bokadeplatser, på http://193.10.202.72/Biljettservice/Bokadeplatser/
                    using (var clientBoka2 = new HttpClient())
                    {
                        clientBoka2.BaseAddress = new Uri("http://193.10.202.72/Biljettservice/");
                        var responsePlatser = clientBoka2.PostAsJsonAsync("Bokadeplatser", nyPlatser).Result;
                    }


                }
            }
            catch (Exception)
            {
                Logger.Error("Error, lyckades ej boka.");
                return View();
            }
                //Tempdata sparas för en alert, notis till användaren
            TempData["boka"] = "Din bokning är nu genomförd.";
            return RedirectToAction("Index", "Home");

            }
            catch (Exception)
            {
                Logger.Error("Error, fel med parse.");
                return View();
            }
        }

    }
}
 