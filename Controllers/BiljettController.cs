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
    public class BiljettController : Controller
    {
        // GET: Biljett
        string Baseurl = "http://193.10.202.72/Biljettservice/";
        public async Task<ActionResult> Index()
        {

            List<BokadePlatser> EmpInfo = new List<BokadePlatser>();
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
                        var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        EmpInfo = JsonConvert.DeserializeObject<List<BokadePlatser>>(EmpResponse);

                    }
                }
            }
            catch (Exception)
            {

                Session["Felhantering"] = "Du är inte uppkopplad mot API:n";


                return RedirectToAction("Index", "Biljett");



            }
            //returning the employee list to view  
            return View(EmpInfo);
        }
    }
}
 