//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ajanvarausprojekti.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Kayttajatunnukset
    {
        public int kayttajatunnus_id { get; set; }

        [Required(ErrorMessage = "Anna k�ytt�j�tunnus!")]
        public string kayttajatunnus { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Anna salasana!")]
        public string salasana { get; set; }
        //[Compare("salasana", ErrorMessage = "Annetut eiv�t vastaa toisiaan.")]
        //[DataType(DataType.Password)]
        //public string vahvistaSalasana { get; set; }
        public int opettaja_id { get; set; }
        public int oikeudet_id { get; set; }

        public string opettaja_nimi { get; set; }

        public string LoginErrorMessage { get; set; }

        public virtual Yllapitooikeudet Yllapitooikeudet { get; set; }
        public virtual Opettajat Opettajat { get; set; }
    }
}
