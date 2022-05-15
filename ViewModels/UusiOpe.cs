using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Ajanvarausprojekti.ViewModels
{
    public class UusiOpe
    {
        public int opettaja_id { get; set; }
        [Required(ErrorMessage = "Etunimi on annettava")]
        public string etunimi { get; set; }
        [Required(ErrorMessage = "Sukunimi on annettava")]
        public string sukunimi { get; set; }
        [Required(ErrorMessage = "Nimike on annettava")]
        public string nimike { get; set; }
        [Required(ErrorMessage = "Sähköposti on annettava")]
        public string sahkoposti { get; set; }
        public Image opeimage { get; set; }

        [Required(ErrorMessage = "Anna käyttäjätunnus!")]
        [StringLength(50, ErrorMessage = "Käyttäjätunnus ei voi olla yli 50 merkkiä pitkä!")]
        public string kayttajatunnus { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Anna salasana!")]
        public string salasana { get; set; }
        public int oikeudet_id { get; set; }
        public int kayttajatunnus_id { get; set; }
    }
}