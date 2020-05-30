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
        // GET: Biljett
        string Baseurl = "http://193.10.202.72/Biljettservice/";
        [Authorize]
        public async Task<ActionResult> Boka(int id)
        {
            BokadePlatser Bokning = new BokadePlatser();

            List<BokadePlatser> BokadePlatser = new List<BokadePlatser>();
            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("Bokadeplatser");

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {

                        //Storing the response details recieved from web api   
                        var BokadePlatserSvar = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        BokadePlatser = JsonConvert.DeserializeObject<List<BokadePlatser>>(BokadePlatserSvar);
                        //Söker ut alla bokade platser där visninschema stämmer med id, får en bokning
                        Bokning = BokadePlatser.Where(b => b.VisningsSchemaId == id).FirstOrDefault();
                    }
                }
            }
            catch (Exception)
            {

                Session["Felhantering"] = "Du är inte uppkopplad mot API:n";


                return RedirectToAction("Index", "Biljett");



            }
            //Sparar undan id i tempdata, för att senare skicka detta för bokningarna
            TempData["tempSchemaId"] = id;
            //Skickar bokningen till sidan
            return View(Bokning);
        }

        //string BaseurlSchema = "http://193.10.202.71/Filmservice/film";
        [Authorize]
        public async Task<ActionResult> VisningsSchema(string titel)
        {
            VisningsSchema Schema = new VisningsSchema();

            List<VisningsSchema> SchemaLista = new List<VisningsSchema>();
            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("VisningsSchema");

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {

                        //Storing the response details recieved from web api   
                        var SchemaResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        SchemaLista = JsonConvert.DeserializeObject<List<VisningsSchema>>(SchemaResponse);
                        //Schema = SchemaLista.Where(s => s.FilmTitel == titel).First();
                        //KundInfo = results.Where(e => e.InloggningsId == inloggningsId).ToList();
                        SchemaLista = SchemaLista.Where(e => e.FilmTitel == titel).ToList();

                    }
                }
            }
            catch (Exception)
            {

                //Session["Felhantering"] = "Du är inte uppkopplad mot API:n";


                return RedirectToAction("Index", "Film");

            }
            //returning the list to view  
            return View(SchemaLista);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Boka(string antalIn)    //Här uppstår ett problem. Textrutan som vi skickar in, returnerar en null? Även om man matat in något?
                                                    //Behöver ändras i inparametern här, beroende på input fältets typ
        {
            int antal = int.Parse(antalIn);

            //Hämta när man ska köpa biljetter
            Kund testKund = null;

            if (Session["Kund"] != null)
            {
                testKund = (Kund)Session["Kund"];
                Console.Write("Success");
            }

            int schema = int.Parse(TempData["tempSchemaId"].ToString());

            //visningsid och antalbokadeplatser
            BokadePlatser nyPlatser = new BokadePlatser { AntalBokadePlatser = antal, VisningsSchemaId = schema };
            //Bokning testBoka = new Bokning { KundId = , VisningsSchemaId = schema };//KundId skall bli ett värde från sessionen
            Bokningar nyBokning = new Bokningar { VisningsSchemaId = schema, KundId = testKund.InloggningsId };

            Console.WriteLine("äpple");



            //Loopa i genom för de bokningar som skall skickas till http://193.10.202.72/BiljettService/Bokningar/
            //Loop är antalet bokade platser, skall ha VisningsSchemaId och KundId, datamodellen säger samma
            using (var clientBoka = new HttpClient())
            {
                clientBoka.BaseAddress = new Uri("http://193.10.202.72/BiljettService/");
                for (int i = 0; i < antal; i++)
                {
                    clientBoka.PostAsJsonAsync("Bokningar", nyBokning);
                    //var response = client.PostAsJsonAsync("Bokningar", testBoka).Result;
                }
                //Efter loopen, skicka något till Bokadeplatser, på http://193.10.202.72/Biljettservice/Bokadeplatser/
                //Ser att de skall ha VisningsSchemaId, AntalBokadePlatser, datamodellen säger VisningsSchemaId och TillgandligaPlatser
                //Klass notis?
                using (var clientBoka2 = new HttpClient())
                {
                    clientBoka2.BaseAddress = new Uri("http://193.10.202.72/Biljettservice/");
                    clientBoka2.PostAsJsonAsync("Bokadeplatser", nyPlatser);
                }


            }

            return RedirectToAction("Index", "Home");
        }

    }
}
 