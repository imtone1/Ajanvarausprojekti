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
        public ActionResult Create()
        {
            //luodaan muuttuja kokonimi, jonka avulla saadaan Views/Palaute/Create- tiedoston dropdown-listaan näkymään opettajan koko nimi

            var kokonimi = db.Opettajat;
            IEnumerable<SelectListItem> selectNimiList = from k in kokonimi
                                                         select new SelectListItem
                                                         {
                                                             Value = k.opettaja_id.ToString(),
                                                             Text = k.etunimi + " " + k.sukunimi
                                                         };
            ViewBag.Kokonimi = new SelectList(selectNimiList, "Value", "Text");

            //ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi");
            ViewBag.palautetyyppi_id = new SelectList(db.Palautetyypit, "palautetyyppi_id", "palautetyyppi");

            return View();
        }

        // POST: Palaute/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "palaute_id,palaute,palautetyyppi_id,opettaja_id")] Palautteet palautteet)
        {
            if (palautteet.palaute != null)
            {
                if (ModelState.IsValid)
                {
                    //Talletetaan palautteen antohetkeksi tämän hetken kellonaika
                    palautteet.palaute_pvm = DateTime.Now;

                    //Tallentaa muutokset tietokantaan
                    db.Palautteet.Add(palautteet);
                    db.SaveChanges();

                    //Annetaan tieto onnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                    TempData["Success"] = "Paljon kiitoksia palautteestasi!";
                    return RedirectToAction("Index", "Home");

                }
                ViewBag.Kokonimi = new SelectList(db.Opettajat, "etunimi", "sukunimi", palautteet.opettaja_id);
                //ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi", palautteet.opettaja_id);
                ViewBag.palautetyyppi_id = new SelectList(db.Palautetyypit, "palautetyyppi_id", "palautetyyppi", palautteet.palautetyyppi_id);


                //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                TempData["Error"] = "Hups! Jokin meni nyt pieleen!";
                return RedirectToAction("Index", "Home");

                //return View(palautteet);
            }
            else
            {
                ViewBag.Kokonimi = new SelectList(db.Opettajat, "etunimi", "sukunimi", palautteet.opettaja_id);
                // ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi", palautteet.opettaja_id);
                ViewBag.palautetyyppi_id = new SelectList(db.Palautetyypit, "palautetyyppi_id", "palautetyyppi", palautteet.palautetyyppi_id);


                //Annetaan tieto epäonnistuneesta palautteesta TempDatalle modaali-ikkunaa varten
                TempData["Error"] = "Hups! Jokin meni nyt pieleen!";
                return RedirectToAction("Index", "Home");

                //return View(palautteet);
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

        public ActionResult _LueKaikki()
        {
            List<Palautteet> model = db.Palautteet.ToList();
            return PartialView("_LueKaikki", model);
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

        //public ActionResult Modal()
        //{
        //    var palautteet = _context.Palautteet.ToList();
        //    ViewBag.Palautteet = palautteet;
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult _Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    return PartialView("_Details");
        //}

        public ActionResult _Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.Palautteet;
            Palautteet palautteet = db.Palautteet.Find(id);
            if (palautteet == null) return HttpNotFound();
            ViewBag.palaute_id = new SelectList(db.Palautteet);
            return PartialView("_Details", palautteet);
        }



        // GET: Palaute/Delete/5
        public ActionResult _Delete(int? id)
        {

            aikapalauteEntities db = new aikapalauteEntities();

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Palautteet palautteet = db.Palautteet.Find(id);
            if (palautteet == null) return HttpNotFound();
            return PartialView("_Delete", palautteet);
        }

        // POST: Palaute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            aikapalauteEntities db = new aikapalauteEntities();

            Palautteet palautteet = db.Palautteet.Find(id);
            db.Palautteet.Remove(palautteet);
            db.SaveChanges();
            return RedirectToAction("Index");
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
