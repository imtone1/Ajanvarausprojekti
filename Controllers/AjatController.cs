using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
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

                            //Annetaan tieto epäonnistuneesta lisäyksestä TempDatalle modaali-ikkunaa varten
                            TempData["Errori"] = "Tälle ajalle on jo ohjausaika!";
                            TempData["BodyText1"] = "Ohjausajan lisäystä ei voi tehdä. Valitse uusi aika.";
                            //return RedirectToAction("OpettajienSivu", "Home");

                            //Tämä ohjaus oikeaan kohtaan (eli uuden ajan lisäykseen) toimisi, mutta errormodaali ehtii tulla väliin 
                            //return new RedirectResult(Url.Action("OpettajienSivu", "Home") + "#LisaaAika");
                        }
                        //Ennen kuin siirrytään silmukassa tarkastelemaan seuraavaa aikaa,
                        //poistetaan aikojen kestolistalta listan ensimmäinen int
                        kestoLista.RemoveAt(0);

                    }
                    //Jos tullaan ulos silmukasta, ohjausajan lisäyksen voi tehdä
                    db.Ajat.Add(ajat);
                    db.SaveChanges();

                    //Annetaan tieto onnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                    TempData["Successi"] = "Ohjausajan lisäys onnistui!";
                    return RedirectToAction("OpettajienSivu", "Home");

                }
                //Annetaan tieto epäonnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                TempData["BodyText1"] = "Ohjausajan lisäys epäonnistui.";
                return RedirectToAction("OpettajienSivu", "Home");
            }
            catch
            {
                //Annetaan tieto epäonnistuneesta ohjausajan lisäyksestä TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                TempData["BodyText1"] = "Ohjausajan lisäys epäonnistui.";
                return RedirectToAction("OpettajienSivu", "Home");
            }
        }

        //modal editin metodi
        public ActionResult _ModalEdit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.Ajat;
            Ajat ajat = db.Ajat.Find(id);
            if (ajat == null) return HttpNotFound();
            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto", selectedValue: "kesto_id");
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", selectedValue: "opettaja_id");
            return PartialView("_ModalEdit", ajat);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598

        public ActionResult _ModalEdit([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id, startDate, startTime")] Ajat ajat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ajat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return PartialView("_ModalEdit", ajat);
        }

        //Modal delete
        // GET: Palaute/Delete/5
        public ActionResult _ModalDelete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Ajat ajat = db.Ajat.Find(id);
            if (ajat == null) return HttpNotFound();
            return PartialView(ajat);
        }

        // POST: Palaute/Delete/5
        [HttpPost, ActionName("_ModalDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ajat ajat = db.Ajat.Find(id);
            db.Ajat.Remove(ajat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _OhjausALista()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Sessiosta otetaan kirjautuneen opettajan id
                var opeid = Session["OpettajaID"];
                int opeOikID = int.Parse(opeid.ToString());
                //Näkyy kirjautuneen opettajan palautteet
                var ohjausALista = from a in db.Ajat
                                   join o in db.Opettajat on a.aika_id equals o.opettaja_id
                                   where o.opettaja_id == opeOikID

                                   select new ohjausAListaData
                                   {
                                       aika_id = (int)a.aika_id,
                                       alku_aika = (DateTime)a.alku_aika,
                                       kesto_id = a.kesto_id,
                                       opettaja_id = (int)o.opettaja_id,
                                   };
                return PartialView("_OhjausALista", ohjausALista);
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