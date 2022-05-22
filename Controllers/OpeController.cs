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

        public ActionResult _VapaatAjat()
        {
            var ajatLista = from a in db.Ajat
                            join op in db.Opettajat on a.opettaja_id equals op.opettaja_id
                            join k in db.Kestot on a.kesto_id equals k.kesto_id
                            join v in db.Varaukset on a.aika_id equals v.aika_id
                            //into al
                            //from varaus in al.DefaultIfEmpty()
                                // where-lause  opettaja id tähän jos halutaan, että tietyn opettajan ajat näkyisivät
                            orderby a.alku_aika

                            select new ajatListaData
                            {
                                aika_id = (int)a.aika_id,
                                Alkuaika = (DateTime)a.alku_aika,
                                Kesto = (int)k.kesto,
                                opettaja_id = (int)a.opettaja_id,

                            };
            return PartialView("_VapaatAjat", ajatLista);
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