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
    
    public partial class Varaukset
    {
        public int varaus_id { get; set; }
        public string varaaja_nimi { get; set; }
        public System.DateTime varattu_pvm { get; set; }
        public int aika_id { get; set; }
    
        public virtual Ajat Ajat { get; set; }
    }
}