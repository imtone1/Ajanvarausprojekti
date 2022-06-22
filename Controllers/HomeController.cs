using Ajanvarausprojekti.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class HomeController : Controller
    {
        private Yhteystiedot ohjelmanyhteystiedot = new Yhteystiedot();
        
        public ActionResult Index()
        {
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
            ViewBag.LoginError = 0;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Yhteystiedot()
        {
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
            //Careerian Tivi-ohjaus-palvelun ylläpidosta vastaa 
            ViewBag.YllapitoyhtTiedot = ohjelmanyhteystiedot.YllapitoyhtTiedot;
        //mailto:
        ViewBag.EmailYhteystiedot = ohjelmanyhteystiedot.EmailYhteystiedot;
        //puh.nro Yhteystiedot sivulla
        ViewBag.Puhnro = ohjelmanyhteystiedot.Puhnro;
        ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Tietosuoja()
        {
            //yhteystiedot
            
            ViewBag.Rekisterinpitaja = ohjelmanyhteystiedot.Rekisterinpitaja;
            ViewBag.Rekisterinpitäjan_yhteyshenkilo = ohjelmanyhteystiedot.Rekisterinpitäjan_yhteyshenkilo;
            ViewBag.Henkilorekisterin_vastuuhenkilo = ohjelmanyhteystiedot.Henkilorekisterin_vastuuhenkilo;
            ViewBag.Tietosuojavastaava = ohjelmanyhteystiedot.Tietosuojavastaava;
            ViewBag.Poistointervalli = Math.Abs(ohjelmanyhteystiedot.PoistoIntervalli);


            return View();
        }

        public ActionResult Kayttoehdot()
        {
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
            return View();
        }

        public ActionResult OpettajienSivu()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
    }
}