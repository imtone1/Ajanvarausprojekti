using System;
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

            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id");
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti");

            return View();
        }


        // POST: Ohjausaika/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id")] Ajat ajat)
        {
            if (ModelState.IsValid)
            {
                db.Ajat.Add(ajat);
                db.SaveChanges();
                return RedirectToAction("LisaaAika", "Ajat");
            }

            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
            return View(ajat);
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