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
    
    public partial class Ajat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ajat()
        {
            this.Varaukset = new HashSet<Varaukset>();
        }
    
        public int aika_id { get; set; }
        public System.DateTime alku_aika { get; set; }
        public int kesto_id { get; set; }
        public int opettaja_id { get; set; }
        public string aihe { get; set; }
        public string paikka { get; set; }
    
        public virtual Kestot Kestot { get; set; }
        public virtual Opettajat Opettajat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Varaukset> Varaukset { get; set; }
        public DateTime startDate { get; set; }
        public DateTime startTime { get; set; }
    }
}
