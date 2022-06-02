using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ajanvarausprojekti.Services
{
    public class Yhteystiedot
    {
        //Nämä gmail osoitteita, koska server gmail
        public string OhjelmanSahkopostiosoite = "tivisovellus@gmail.com";
        public string OhjelmanSpostiSalasana = "kxgkmvavjrqoeubz";

        //default opekuva, tämä on vain polku eli tämän kuvan pitäisi olla lisättynä Opekuva -tiedostoon ensin
        public string OpeDefaultKuva = "/Opekuvat/defaultKuva.jpg";
    }
}