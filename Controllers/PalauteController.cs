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

        //vapautetaan olio lopussa omalla metodillaan. Tällöin sitä ei tarvitse vapauttaa jokaisessa metodissa erikseen.

        // GET: Palaute
        public ActionResult Index()
        {

            //Listataan kaikki palautteet Palaute-näkymän Index-sivulle (Vews-Palaute-Index)
            List<Palautteet> model = db.Palautteet.ToList();
            return View(model);
        }

        // GET: Palaute/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

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


            //luodaan uusi palautteen aiheeseen liittyvä lista, jonka avulla saadaan pudotusvalikkoon näkymään oletusteksti "Valitse aihe..."

            List<SelectListItem> palauteListItems = db.Palautetyypit.Select(p => new SelectListItem()
            {
                Text = p.palautetyyppi,
                Value = p.palautetyyppi_id.ToString()
            }).ToList();

            palauteListItems.Insert(0, new SelectListItem() { Text = "Valitse aihe...", Value = "0", Selected = true });
            ViewBag.PalauteSelectList = palauteListItems;

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
                    TempData["Errori"] = "Palautteen lähetys epäonnistui.";
                    return RedirectToAction("Index", "Home");

                }
                else
                {

                    //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Palautteen lähetys epäonnistui.";
                    return RedirectToAction("Index", "Home");

                }
            }
            catch
            {
                //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Palautteen lähetys epäonnistui.";
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Palaute/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Palaute/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Palaute/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Palaute/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var palauteLista = from p in db.Palautteet
                               join pt in db.Palautetyypit on p.palautetyyppi_id equals pt.palautetyyppi_id
                               join op in db.Opettajat on p.opettaja_id equals op.opettaja_id

                               orderby p.palaute_id

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
            ViewBag.palaute_id = new SelectList(db.Palautteet);
            return PartialView("_Details", palauteLista);
        }



        // GET: Palaute/Delete/5
        public ActionResult _Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Palautteet palautteet = db.Palautteet.Find(id);
            if (palautteet == null) return HttpNotFound();
            return PartialView(palautteet);
        }

        // POST: Palaute/Delete/5
        [HttpPost, ActionName("_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Palautteet palautteet = db.Palautteet.Find(id);
            db.Palautteet.Remove(palautteet);
            db.SaveChanges();
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
