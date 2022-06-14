using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.ViewModels;

namespace Ajanvarausprojekti.Controllers
{
    public class OpeController : Controller
    {

        //Luodaan tietokantayhteys ylätasolla jolloin se näkyy jokaiseen metodiin
        aikapalauteEntities db = new aikapalauteEntities();
        //vapautetaan olio lopussa omalla metodillaan. Tällöin sitä ei tarvitse vapauttaa jokaisessa metodissa erikseen.

        // GET: Ope
        public ActionResult Index()
        {
            List<Opettajat> model = db.Opettajat.ToList();
            return View(model);
        }
        

        public ActionResult Opekortit()
        {
            List<Opettajat> model = db.Opettajat.ToList();
            return PartialView("Opekortit", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}