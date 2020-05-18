using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ja.Controllers
{
    public class BiljettController : Controller
    {
        // GET: Biljett
        public ActionResult Index()
        {
            return View();
        }
    }
}