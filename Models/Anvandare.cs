﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ja.Models
{
    public class Anvandare
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Losenord { get; set; }
        public int Behorighetsniva { get; set; }
    }
}