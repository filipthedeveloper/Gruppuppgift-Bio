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
        // GET: Film
        

            //Hosted web API REST Service base url  
            string Baseurl = "http://193.10.202.71/";
            public async Task<ActionResult> Index()
            {
                try
                {
                    List<Filmer> Filmlista = new List<Filmer>();



                    using (var client = new HttpClient())
                    {
                        //Passing service base url  
                        client.BaseAddress = new Uri(Baseurl);



                        client.DefaultRequestHeaders.Clear();
                        //Define request data format  
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



                        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                        HttpResponseMessage Res = await client.GetAsync("/Filmservice/film");



                        //Checking the response is successful or not which is sent using HttpClient  
                        if (Res.IsSuccessStatusCode)
                        {
                            //Storing the response details recieved from web api   
                            var EmpResponse = Res.Content.ReadAsStringAsync().Result;



                            //Deserializing the response recieved from web api and storing into the Employee list  
                            Filmlista = JsonConvert.DeserializeObject<List<Filmer>>(EmpResponse);



                        }
                        //returning the employee list to view  
                        return View(Filmlista);
                    }
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", "Uppgifterna kunde inte sparas. " + ex.Message);
                    Debug.WriteLine("Något gick fel med uppkopplingen. " + ex.Message);
                    //Console.WriteLine("Något gick fel med uppkopplingen. " + ex.Message);
                }
                return RedirectToAction("Index", "Film");



            }



            //// GET: Api
            //public ActionResult Index()
            //{
            //    return View();
            //}
        }

    }


