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
    
    public partial class Opettajat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Opettajat()
        {
            this.Ajat = new HashSet<Ajat>();
            this.Kayttajatunnukset = new HashSet<Kayttajatunnukset>();
            this.Palautteet = new HashSet<Palautteet>();
        }
    
        public int opettaja_id { get; set; }
        public string sahkoposti { get; set; }
        public string etunimi { get; set; }
        public string sukunimi { get; set; }
        public string nimike { get; set; }
        public string kuva { get; set; }
        public byte[] opeimage { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ajat> Ajat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kayttajatunnukset> Kayttajatunnukset { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Palautteet> Palautteet { get; set; }
    }
}
