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
    public class PalauteController : Controller
    {
        //Luodaan tietokantayhteys ylätasolla jolloin se näkyy jokaiseen metodiin
        aikapalauteEntities db = new aikapalauteEntities();
        //vapautetaan olio lopussa omalla metodillaan.


        // GET: Palaute/Create
        public ActionResult _Create()
        {
            //luodaan uusi opettajan valintaan liittyvä lista, jonka avulla saadaan pudotusvalikkoon näkymään oletusteksi "Valitse opettaja..."
            //lista myös näyttää open etunimi+sukunimi
            List<SelectListItem> opetListItems = db.Opettajat.Select(o => new SelectListItem()
            {
                Text = o.etunimi + " " + o.sukunimi,
                Value = o.opettaja_id.ToString(),
            }).ToList();
            opetListItems.Insert(0, new SelectListItem() { Text = "Valitse opettaja...", Value = "0", Selected = true });
            ViewBag.opeSelectList = opetListItems;

            return PartialView();
        }

        // POST: Palaute/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create([Bind(Include = "palaute_id,palaute,palautetyyppi_id,opettaja_id")] Palautteet palautteet)
        {
            try
            {
                if (palautteet.palaute != null && palautteet.opettaja_id != 0 && palautteet.palautetyyppi_id != 0)
                {
                    if (ModelState.IsValid)
                    {
                        //Talletetaan palautteen antohetkeksi tämän hetken kellonaika
                        palautteet.palaute_pvm = DateTime.Now;

                        //Tallentaa muutokset tietokantaan
                        db.Palautteet.Add(palautteet);
                        db.SaveChanges();

                        //Annetaan tieto onnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                        TempData["Successi"] = "Paljon kiitoksia palautteestasi!";
                        return RedirectToAction("Index", "Home");

                    }
                    //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                    TempData["BodyText1"] = "Palautteen lähetys epäonnistui.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Kirjoita ensin palaute!";
                    TempData["BodyText1"] = "Palautteen lähetys epäonnistui.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                TempData["BodyText1"] = "Palautteen lähetys epäonnistui.";
                TempData["BodyText2"] = "Yritä hetken kuluttua uudelleen.";
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult LueKaikki()
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
                var palauteLista = from p in db.Palautteet
                                   join pt in db.Palautetyypit on p.palautetyyppi_id equals pt.palautetyyppi_id
                                   join op in db.Opettajat on p.opettaja_id equals op.opettaja_id
                                   where op.opettaja_id == opeOikID

                                   orderby p.palaute_pvm

                                   select new palauteListaData
                                   {
                                       palaute_id = (int)p.palaute_id,
                                       palaute_pvm = (DateTime)p.palaute_pvm,
                                       palautetyyppi_id = p.palautetyyppi_id,
                                       palautetyyppi = pt.palautetyyppi,
                                       palaute = p.palaute,
                                       opettaja_id = (int)op.opettaja_id,
                                       sukunimi = op.sukunimi,
                                       etunimi = op.etunimi,
                                   };
                return View("LueKaikki", palauteLista);
            }
        }


        public ActionResult _LueViisi()
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
                var palauteLista = from p in db.Palautteet
                                   join pt in db.Palautetyypit on p.palautetyyppi_id equals pt.palautetyyppi_id
                                   join op in db.Opettajat on p.opettaja_id equals op.opettaja_id
                                   where op.opettaja_id == opeOikID

                                   orderby p.palaute_pvm

                                   select new palauteListaData
                                   {
                                       palaute_id = (int)p.palaute_id,
                                       palaute_pvm = (DateTime)p.palaute_pvm,
                                       palautetyyppi_id = p.palautetyyppi_id,
                                       palautetyyppi = pt.palautetyyppi,
                                       palaute = p.palaute,
                                       opettaja_id = (int)op.opettaja_id,
                                       sukunimi = op.sukunimi,
                                       etunimi = op.etunimi,
                                   };
                return PartialView("_LueViisi", palauteLista);
            }
        }


        public ActionResult _Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Palautteet palautteet = db.Palautteet.Find(id);
            if (palautteet == null)
            {
                return HttpNotFound();
            }
            ViewBag.palaute_id = new SelectList(db.Palautteet, "palaute_id", "palaute", palautteet.palaute_id);
            ViewBag.palaute_id = new SelectList(db.Palautetyypit, "palaute_id", "palautetyyppi", palautteet.palaute_id);
            return PartialView("_Details", palautteet);
        }


        // GET: Palaute/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Palautteet palautteet = db.Palautteet.Find(id);
            if (palautteet == null) return HttpNotFound();
            return View(palautteet);
        }

        // POST: Palaute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Palautteet palautteet = db.Palautteet.Find(id);
            db.Palautteet.Remove(palautteet);
            db.SaveChanges();
            //Annetaan tieto onnistuneesta palautteen poistosta TempDatalle modaali-ikkunaa varten
            TempData["Successi"] = "Palautteen poisto onnistui!";
            return RedirectToAction("LueKaikki");
        }


        //vapautetaan tietokantayhteys Disposella
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
