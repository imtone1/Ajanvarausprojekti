using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Ajanvarausprojekti.ViewModels
{
    public class ohjausAListaData
    {
        [Key]
        public int aika_id { get; set; }
        public System.DateTime alku_aika { get; set; }
        public int kesto_id { get; set; }
        public int opettaja_id { get; set; }
        public string aihe { get; set; }
        public string paikka { get; set; }
        public Nullable<int> kesto { get; set; }

    }
}