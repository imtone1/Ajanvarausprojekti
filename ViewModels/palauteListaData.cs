using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Ajanvarausprojekti.ViewModels
{
    public class palauteListaData
    {
        public int opettaja_id { get; set; }
        [Required(ErrorMessage = "Etunimi on annettava")]
        public string etunimi { get; set; }
        [Required(ErrorMessage = "Sukunimi on annettava")]
        public string sukunimi { get; set; }
        [Required(ErrorMessage = "Nimike on annettava")]
        public int palaute_id { get; set; }
        public string palautetyyppi { get; set; }
        public string palaute { get; set; }
        public System.DateTime palaute_pvm { get; set; }
        public Nullable<int> palautetyyppi_id { get; set; }

    }
}