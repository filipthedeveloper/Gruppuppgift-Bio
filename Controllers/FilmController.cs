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

namespace Ja.Controllers
{
    public class FilmController : Controller
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //WEB API url
        string Baseurl = "http://193.10.202.71/Filmservice/film";
        public async Task<ActionResult> Index()
        {

            List<Filmer> Filmlist = new List<Filmer>();
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
                    HttpResponseMessage Res = await client.GetAsync("film");

                    //Koll om hämtning av filmer lyckades  
                    if (Res.IsSuccessStatusCode)
                    {

                        //Sparar undan svaret
                        var Filmresponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing på svaret till en lista
                        Filmlist = JsonConvert.DeserializeObject<List<Filmer>>(Filmresponse);

                    }
                }
            }
            catch (Exception)
            {
                Logger.Error("Error, kunde ej hämta filmer.");
                return RedirectToAction("Index", "Film");
            }
            //Returnerar informationen till vyn
            return View(Filmlist);
        }
    }
}


