using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ja.Models
{
    public class VisningsSchema
    {
        public int Id { get; set; }
        public string FilmTitel { get; set; }
        public string SalongsNamn { get; set; }
        public DateTime Visningstid { get; set; }
    }
}