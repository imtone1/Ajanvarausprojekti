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
            //luodaan uusi ohjausajan keston valintaan liittyvä lista, jonka avulla saadaan pudotusvalikkoon näkymään oletusteksi "Valitse kesto..."

            List<SelectListItem> kestoListItems = db.Kestot.Select(k => new SelectListItem()
            {
                Text = k.kesto.ToString(),
                Value = k.kesto_id.ToString()
            }).ToList();
            kestoListItems.Insert(0, new SelectListItem() { Text = "Valitse kesto...", Value = null, Selected = true });
            ViewBag.kestoSelectList = kestoListItems;

            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id");
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti");

            return View();
        }


        // POST: Ohjausaika/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LisaaAika([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id")] Ajat ajat)
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