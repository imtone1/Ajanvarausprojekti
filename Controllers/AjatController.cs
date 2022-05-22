﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajanvarausprojekti.Models;

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
        public ActionResult LisaaAika()
        {
            if (Session["OpettajaID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {

                ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id");
                ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti");

                return View();

            }
        }


        // POST: Ohjausaika/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id")] Ajat ajat)
        public ActionResult LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id, startDate, startTime")] Ajat ajat)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    
                    //Opettajan valitsema päivämäärä on muuttujassa startDate
                    var startDate = Request["startDate"];
                    //Opettajan valitsema kellonaika on muuttujassa startTime
                    var startTime = Request["startTime"];
                    //Yhdistetään nen samaan muuttujaan
                    var valittuAika = startDate + " " + startTime;
                    //Convertoidaan tietokantaan sopivaksi
                    ajat.alku_aika = Convert.ToDateTime(valittuAika);

                    //Varauksen tehnyt opettaja tallennetaan tietokantaan Session["OpettajaID"]:n avulla
                    ajat.opettaja_id = (int)Session["OpettajaID"];




                    ////Tarkistetaan, onko opettajalla jo olemassa kyseinen aika
                    //var testForAika = from a in db.Ajat
                    //                  where a.opettaja_id == (int)Session["OpettajaID"] && a.alku_aika == Convert.ToDateTime(valittuAika)
                    //                  select a;

                    //if (testForAika.Any())
                    //{
                    //    //Annetaan tieto epäonnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                    //    TempData["Errori"] = "Tälle ajankohdalle on jo lisätty ohjausaika.";
                    //    TempData["BodyText1"] = "Valitse uusi aika.";
                    //    return RedirectToAction("LisaaAika", "Ajat");
                    //}

                    //else
                    //{
                        

                    //}


                    db.Ajat.Add(ajat);
                    db.SaveChanges();

                    //Annetaan tieto onnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                    TempData["Successi"] = "Ohjausajan lisäys onnistui!";
                    return RedirectToAction("LisaaAika", "Ajat");


                }

                //ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
                //ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
                //return View(ajat);

                //Annetaan tieto epäonnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                TempData["BodyText1"] = "Ohjausajan lisäys epäonnistui.";
                return RedirectToAction("LisaaAika", "Ajat");

            }
            catch
            {
                //Annetaan tieto epäonnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                TempData["BodyText1"] = "Ohjausajan lisäys epäonnistui.";
                return RedirectToAction("LisaaAika", "Ajat");
            }



        }


        public ActionResult Edit(int id)
        {
            return View();
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