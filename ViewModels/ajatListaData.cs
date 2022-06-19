namespace Ajanvarausprojekti.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class ajatListaData
    {
        public int aika_id { get; set; }
        public Nullable<System.DateTime> Alkuaika { get; set; }
        public int kesto_id { get; set; }
        public int opettaja_id { get; set; }
        public string Aihe { get; set; }
        public string Paikka { get; set; }
        public int varaus_id { get; set; }
        [Required(ErrorMessage = "Anna sähköpostiosoitteesi tai nimesi")]
        public string Varaaja { get; set; }
        public Nullable<System.DateTime> Varauspvm { get; set; }

        public int Kesto { get; set; }

        public string id_hash { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }

    }
}