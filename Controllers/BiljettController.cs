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
            //Skickar bokningen till sidan
            return View(Bokning);
        }

        //string BaseurlSchema = "http://193.10.202.71/Filmservice/film";
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
                        Schema = SchemaLista.Where(s => s.FilmTitel == titel).FirstOrDefault();

                    }
                }
            }
            catch (Exception)
            {

                Session["Felhantering"] = "Du är inte uppkopplad mot API:n";


                return RedirectToAction("Index", "Film");



            }
            //returning the employee list to view  
            return View(Schema);
        }

        [HttpPost]
        public ActionResult Boka(string antalIn, int schema)
        {
            int antal = Int32.Parse(antalIn);

            //visningsid och antalbokadeplatser
            BokadePlatser testPlatser = new BokadePlatser { AntalBokadePlatser = antal, VisningsSchemaId = schema };
            //Bokning testBoka = new Bokning { KundId = , VisningsSchemaId = schema };//KundId skall bli ett värde från sessionen

            Console.WriteLine("äpple");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://193.10.202.72/BiljettService/");
                for (int i = 0; i < antal; i++)
                {
                    //var response = client.PostAsJsonAsync("Bokningar", testBoka).Result;
                }


            }

            return RedirectToAction("Index", "Home");
        }

    }
}
 