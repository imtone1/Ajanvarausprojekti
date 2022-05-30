using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult VapaatAjatKoko()
        {
            // LIstataan kaikki kyseisen opettajan ajat

            var ajatLista = (from a in db.Ajat
                            join o in db.Opettajat on a.opettaja_id equals o.opettaja_id
                            join k in db.Kestot on a.kesto_id equals k.kesto_id
                            where o.opettaja_id == 4 // Tässä annetaan kovakoodattuna opeid, oikeassa versiossa tulee klikkauksesta jQueryllä
                             where a.alku_aika >= DateTime.Today
                             orderby a.alku_aika

                             select new ajatListaData
                            {
                                Etunimi = o.etunimi,
                                Sukunimi = o.sukunimi,
                                aika_id = (int)a.aika_id,
                                Alkuaika = (DateTime)a.alku_aika,
                                Kesto = (int)k.kesto,
                                opettaja_id = (int)a.opettaja_id,
                                Paikka = a.paikka,
                            }).ToList();
          
            // Listataan kyseisen opettajan varatut ajat

            var varatut = (from a in db.Ajat
                             join o in db.Opettajat on a.opettaja_id equals o.opettaja_id
                             join k in db.Kestot on a.kesto_id equals k.kesto_id
                             join v in db.Varaukset on a.aika_id equals v.aika_id
                             where o.opettaja_id == 4
                             where a.alku_aika >= DateTime.Today
                             where a.aika_id == v.aika_id
                             orderby a.alku_aika
                           

                             select new ajatListaData
                             {
                                 Etunimi = o.etunimi,
                                 Sukunimi = o.sukunimi,
                                 aika_id = (int)a.aika_id,
                                 Alkuaika = (DateTime)a.alku_aika,
                                 Kesto = (int)k.kesto,
                                 opettaja_id = (int)a.opettaja_id,
                                 Paikka = a.paikka,
                             }).ToList();



            // Listataan vapaat ajat poistamalla kaikista ajoista varatut ajat

            var vapaatAjat = (from a in ajatLista
                       where !varatut.Any(x => x.aika_id == a.aika_id)
                       select a).ToList();

            return View("vapaatAjatKoko", vapaatAjat); // Huom! Tässä haarassa palauttaa näkymän, ei kuulu lopulliseen!!
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