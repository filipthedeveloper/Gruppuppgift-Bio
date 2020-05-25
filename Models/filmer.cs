using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ja.Models
{
    public class Filmer
    {
        public int Id { get; set; }

        public string Titel { get; set; }

        public float Speltid { get; set; }

        public string Genre { get; set; }

        public int Aldersgrans { get; set; }

        public string Beskrivning { get; set; }

        public string Sprak { get; set; }

        public string Utgivningsdatum { get; set; }

        public string Huvudregissor { get; set; }

    }
}