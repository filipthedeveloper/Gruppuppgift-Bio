using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ja.Models
{
    public class Bokning
    {
        public int Id { get; set; }
        public int KundId { get; set; }
        public int VisningsSchemaId { get; set; }
    }
}