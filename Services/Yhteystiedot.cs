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


        //sivuston nimi
        public string SivustonNimi = "Careerian Tivi-ohjaus";
        //tietosuoja, käyttäjätiedot, yhteystiedot
        public string Rekisterinpitaja = "Simo Siren, simo.siren@careeria.fi, TÄHÄN PUH";
        public string Rekisterinpitäjan_yhteyshenkilo = "Opettaja Simo Siren";
        public string Henkilorekisterin_vastuuhenkilo = "Opettaja Simo Siren";
        public string Tietosuojavastaava = "Simo Siren, simo.siren @careeria.fi";

        //Careerian Tivi-ohjaus-palvelun ylläpidosta vastaa 
        public string YllapitoyhtTiedot="tivi-opettaja Simo Siren";
        //mailto:
        public string EmailYhteystiedot = "Simo.Siren@careeria.fi";
        //puh.nro Yhteystiedot sivulla
        public string Puhnro = "";

        //määrittelee millä intervallilla poistetaan vanhentuneita varauksia, nyt on 2pv. 
        public int PoistoIntervalli = -2;

        //default opekuva, tämä on vain polku eli tämän kuvan pitäisi olla lisättynä Opekuva -tiedostoon ensin
        public string OpeDefaultKuva = "/Opekuvat/defaultKuva.jpg";
    }
}