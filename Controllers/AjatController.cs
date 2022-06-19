using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.ViewModels;

namespace Ajanvarausprojekti.Controllers
{
    public class AjatController : Controller
    {
        //Luodaan tietokantayhteys ylätasolla jolloin se näkyy jokaiseen metodiin
        aikapalauteEntities db = new aikapalauteEntities();

        // GET: Ajat
        public ActionResult _VapaatAjat()
        {

            DateTime aikatieto;
            TimeZoneInfo timezone;
            timezone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            aikatieto = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
            // LIstataan kaikki kyseisen opettajan ajat

            var opeID = (int)Session["OpettajaID"];
            var ajatLista = (from a in db.Ajat
                             join o in db.Opettajat on a.opettaja_id equals o.opettaja_id
                             join k in db.Kestot on a.kesto_id equals k.kesto_id
                             where o.opettaja_id == opeID
                             where a.alku_aika >= aikatieto

                             select new ajatListaData
                             {
                                 Etunimi = o.etunimi,
                                 Sukunimi = o.sukunimi,
                                 aika_id = (int)a.aika_id,
                                 Alkuaika = (DateTime)a.alku_aika,
                                 Kesto = (int)k.kesto,
                                 opettaja_id = (int)a.opettaja_id,
                                 Paikka = a.paikka
                             }).ToList();

            // Listataan kyseisen opettajan varatut ajat

            var varatut = (from a in db.Ajat
                           join o in db.Opettajat on a.opettaja_id equals o.opettaja_id
                           join k in db.Kestot on a.kesto_id equals k.kesto_id
                           join v in db.Varaukset on a.aika_id equals v.aika_id
                           where o.opettaja_id == opeID
                           where a.aika_id == v.aika_id
                           where a.alku_aika >= aikatieto

                           select new ajatListaData
                           {
                               Etunimi = o.etunimi,
                               Sukunimi = o.sukunimi,
                               aika_id = (int)a.aika_id,
                               Alkuaika = (DateTime)a.alku_aika,
                               Kesto = (int)k.kesto,
                               opettaja_id = (int)a.opettaja_id,
                               Paikka = a.paikka
                           }).ToList();

            // Listataan vapaat ajat poistamalla kaikista ajoista varatut ajat

            var vapaatAjat = (from a in ajatLista
                              where !varatut.Any(x => x.aika_id == a.aika_id)
                              orderby a.Alkuaika
                              select a).ToList();

            return PartialView("_VapaatAjat", vapaatAjat);
        }

        // GET: Ohjausaika/Create
        public ActionResult _LisaaAika()
        {
            if (Session["OpettajaID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {

                ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id");
                ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti");

                return PartialView();

            }
        }

        // POST: Ohjausaika/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id, startDate, startTime, paikka")] Ajat ajat)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //Tuodaan uuden ajan lisäykseen tarvittavat tiedot muuttujiin
                    var startDate = Request["startDate"];
                    var startTime = Request["startTime"];
                    var strAika = startDate + " " + startTime;
                    ajat.alku_aika = Convert.ToDateTime(strAika);
                    var kesto_id = Request["kesto_id"];
                    var paikka = Request["paikka"];

                    //Varauksen tehnyt opettaja tallennetaan tietokantaan Session["OpettajaID"]:n avulla
                    ajat.opettaja_id = (int)Session["OpettajaID"];

                    //Sijoitetaan opettajan antamat arvot apumuuttujiin LINQ-kyselyä varten
                    var valittuAika = Convert.ToDateTime(strAika);
                    var opeID = (int)Session["OpettajaID"];

                    //Haetaan tietokannasta opettajan kaikki samana päivänä vapaana olevat ajat ja kestot
                    var alkuajatLista = db.Ajat.Where(a => a.opettaja_id == opeID && a.alku_aika.Day == valittuAika.Day && a.alku_aika.Month == valittuAika.Month)
                                          .Select(a => a.alku_aika);
                    var kestoListaLinq = db.Ajat.Where(a => a.opettaja_id == opeID && a.alku_aika.Day == valittuAika.Day && a.alku_aika.Month == valittuAika.Month)
                                       .Select(a => a.kesto_id);
                    var kestoLista = kestoListaLinq.ToList();

                    //käydään silmukassa läpi opettajan ajat kyseiselle päivälle
                    foreach (var alkuaika in alkuajatLista)
                    {
                        var kesto = kestoLista[0];
                        DateTime lopetusAika = alkuaika.AddMinutes(kesto);
                        //Vertaillaan valittua aikaa ja tietokannassa olevia aikoja Comparen avulla. Huomioidaan myös ajan kesto.
                        for (int i = 0; i < Int32.Parse(kesto_id); i++)
                        {
                            DateTime aikaTarkistus = valittuAika.AddMinutes(i);
                            int vertailu1 = DateTime.Compare(aikaTarkistus, alkuaika);
                            int vertailu2 = DateTime.Compare(aikaTarkistus, lopetusAika);
                            if (vertailu1 >= 0 && vertailu2 < 0)
                            {
                                //Jos mennään tähän lohkoon, Kyseiselle ajankohdalle on jo laitettu ohjausaika eikä päällekkäisiä aikoja saa olla.
                                TempData["AikaError"] = "Tälle ajankohdalle on jo olemassa ohjausaika, valitse uusi aika.";
                                return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");
                            }
                        }
                        kestoLista.RemoveAt(0);
                    }

                    //Jos tullaan ulos silmukasta, ohjausajan lisäyksen voi tehdä
                    db.Ajat.Add(ajat);
                    db.SaveChanges();

                    TempData["AikaSuccess"] = "Uuden ohjausajan lisäys onnistui!";
                    return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");

                }
                TempData["AikaError"] = "Jotain meni pieleen, ajan lisäys epäonnistui. Yritä uudelleen.";
                return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");
            }
            catch
            {
                TempData["AikaError"] = "Jotain meni pieleen, ajan lisäys epäonnistui. Yritä uudelleen.";
                return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");
            }
        }

        //Delete View
        // GET: Palaute/Delete/5
        public ActionResult AjatDelete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Ajat ajat = db.Ajat.Find(id);
            if (ajat == null) return HttpNotFound();
            return View("AjatDelete", ajat);
        }

        // POST: Palaute/Delete/5
        [HttpPost, ActionName("AjatDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ajat ajat = db.Ajat.Find(id);
            db.Ajat.Remove(ajat);
            db.SaveChanges();
            TempData["AikaSuccess"] = "Ajan poisto onnistui!";
            return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");
        }

        public ActionResult _VapaatAjatOpiskelijalle(int? opeid)
        {

            DateTime aikatieto;
            TimeZoneInfo timezone;
            timezone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            aikatieto = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
            // LIstataan kaikki kyseisen opettajan ajat

            var ajatLista = (from a in db.Ajat
                             join o in db.Opettajat on a.opettaja_id equals o.opettaja_id
                             join k in db.Kestot on a.kesto_id equals k.kesto_id
                             where o.opettaja_id == opeid
                             where a.alku_aika >= aikatieto
                             orderby a.alku_aika

                             select new ajatListaData
                             {
                                 Etunimi = o.etunimi,
                                 Sukunimi = o.sukunimi,
                                 aika_id = (int)a.aika_id,
                                 Alkuaika = (DateTime)a.alku_aika,
                                 Kesto = (int)k.kesto,
                                 opettaja_id = (int)a.opettaja_id,
                                 Paikka = a.paikka
                             }).ToList();

            // Listataan kyseisen opettajan varatut ajat

            var varatut = (from a in db.Ajat
                           join o in db.Opettajat on a.opettaja_id equals o.opettaja_id
                           join k in db.Kestot on a.kesto_id equals k.kesto_id
                           join v in db.Varaukset on a.aika_id equals v.aika_id
                           where o.opettaja_id == opeid
                           where a.alku_aika >= aikatieto
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
                               Paikka = a.paikka
                           }).ToList();

            // Listataan vapaat ajat poistamalla kaikista ajoista varatut ajat

            var vapaatAjat = (from a in ajatLista
                              where !varatut.Any(x => x.aika_id == a.aika_id)
                              select a).ToList();
            if (vapaatAjat.Count() > 0)
            {
                ViewBag.EiVarauksia = "Opettajalla ei ole vapaita aikoja.";
            }
            else
            {
                ViewBag.EiVarauksia = "";
            }


            return PartialView("_VapaatAjatOpiskelijalle", vapaatAjat);
        }


        //vapautetaan lopussa tietokantayhteys Disposella
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