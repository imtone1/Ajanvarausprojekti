﻿using System;
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
        public ActionResult Index()
        {
            return View();
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
        //public ActionResult LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id")] Ajat ajat)
        public ActionResult _LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id, startDate, startTime")] Ajat ajat)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    //Opettajan valitsema päivämäärä on muuttujassa startDate
                    var startDate = Request["startDate"];
                    //Opettajan valitsema kellonaika on muuttujassa startTime
                    var startTime = Request["startTime"];
                    //Yhdistetään nen samaan string-muuttujaan
                    var strAika = startDate + " " + startTime;
                    //Convertoidaan tietokantaan sopivaksi
                    ajat.alku_aika = Convert.ToDateTime(strAika);

                    //Varauksen tehnyt opettaja tallennetaan tietokantaan Session["OpettajaID"]:n avulla
                    ajat.opettaja_id = (int)Session["OpettajaID"];

                    //Sijoitetaan opettajan antamat arvot apumuuttujiin LINQ-kyselyä varten
                    var valittuAika = Convert.ToDateTime(strAika);
                    var opeID = (int)Session["OpettajaID"];

                    //Haetaan tietokannasta opettajan kaikki samana päivänä vapaana olevat ajat
                    var alkuajatLista = db.Ajat.Where(a => a.opettaja_id == opeID && a.alku_aika.Day == valittuAika.Day && a.alku_aika.Month == valittuAika.Month)
                                          .Select(a => a.alku_aika);

                    //Haetaan tietokannasta kaikki samana päivänä vapaana olevien aikojen kestot
                    var kestoListaLinq = db.Ajat.Where(a => a.opettaja_id == opeID && a.alku_aika.Day == valittuAika.Day && a.alku_aika.Month == valittuAika.Month)
                                       .Select(a => a.kesto_id);
                    //sijoitetaan linq-kyselyllä haetut kestot listaan
                    var kestoLista = kestoListaLinq.ToList();

                    foreach (var alkuaika in alkuajatLista)
                    {
                        //annetaan muuttujalle kesto listalla olevan ensimmäisen ohjausajan kesto
                        var kesto = kestoLista[0];

                        //Luodaan muuttuja lopetusAika, joka lasketaan keston perusteella
                        DateTime lopetusAika = alkuaika.AddMinutes(kesto);

                        //Vertaillaan datetimeja Comparen avulla
                        //Ensin vertaillaan valittua aikaa ja alkuaikaa
                        int vertailu1 = DateTime.Compare(valittuAika, alkuaika);
                        //Sitten vertaillaan valittua aikaa ja lopetuaikaa
                        int vertailu2 = DateTime.Compare(valittuAika, lopetusAika);
                        //Comparen palauttama arvo riippuu siitä, onko t1 aiemmin<0, sama=0, myöhemmin>0 kuin t2

                        //tarkastetaan vertailujen palauttamat arvot
                        if (vertailu1 >= 0 && vertailu2 < 0)
                        {
                            //Jos mennään tähän lohkoon, Kyseiselle ajankohdalle on jo laitettu ohjausaika.
                            TempData["AikaError"] = "Tälle ajankohdalle on jo olemassa ohjausaika, valitse uusi aika.";
                            return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");
                        }
                        //Ennen kuin siirrytään silmukassa tarkastelemaan seuraavaa aikaa,
                        //poistetaan aikojen kestolistalta listan ensimmäinen int
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