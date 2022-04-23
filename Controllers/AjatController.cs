using System;
using System.Collections.Generic;
using System.Data;
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
        private aikapalauteEntities db = new aikapalauteEntities();

        // GET: Ajat
        public ActionResult Index()
        {
            var ajat = db.Ajat.Include(a => a.Kestot).Include(a => a.Opettajat);
            return View(ajat.ToList());
        }

        // GET: Ajat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ajat ajat = db.Ajat.Find(id);
            if (ajat == null)
            {
                return HttpNotFound();
            }
            return View(ajat);
        }

        // GET: Ajat/Create
        public ActionResult Create()
        {
            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id");
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti");
            return View();
        }

        // POST: Ajat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id,aihe,paikka")] Ajat ajat)
        {
            if (ModelState.IsValid)
            {
                db.Ajat.Add(ajat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
            return View(ajat);
        }

        // GET: Ajat/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ajat ajat = db.Ajat.Find(id);
            if (ajat == null)
            {
                return HttpNotFound();
            }
            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
            return View(ajat);
        }

        // POST: Ajat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id,aihe,paikka")] Ajat ajat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ajat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
            return View(ajat);
        }

        // GET: Ajat/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ajat ajat = db.Ajat.Find(id);
            if (ajat == null)
            {
                return HttpNotFound();
            }
            return View(ajat);
        }

        // POST: Ajat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ajat ajat = db.Ajat.Find(id);
            db.Ajat.Remove(ajat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AikaListaus()
        {
                //left join, jotta näkyisi kaikki ajat, myös ne joissa ei varausta
                var ajatLista = from a in db.Ajat
                join op in db.Opettajat on a.opettaja_id equals op.opettaja_id
            join k in db.Kestot on a.kesto_id equals k.kesto_id
            join v in db.Varaukset on a.aika_id equals v.aika_id
            into gj from varaus in gj.DefaultIfEmpty()
            // where-lause  opettaja id tähän?
            orderby a.alku_aika

                            select new ajatListaData
                            {
                                aika_id = (int)a.aika_id,
                                Alkuaika = (DateTime)a.alku_aika,
                                Kesto = (int)k.kesto,
                                Aihe = a.aihe,
                                Paikka = a.paikka,
                                opettaja_id = (int)a.opettaja_id,
                                Varaaja = varaus.varaaja_nimi,
                                Varauspvm = (DateTime)varaus.varattu_pvm,

                            };
            return View(ajatLista);
        }

        public ActionResult VarausListaus()
        {
            //left join, jotta näkyisi kaikki ajat, myös ne joissa ei varausta
            var ajatLista = from a in db.Ajat
                            join op in db.Opettajat on a.opettaja_id equals op.opettaja_id
                            join k in db.Kestot on a.kesto_id equals k.kesto_id
                            join v in db.Varaukset on a.aika_id equals v.aika_id
                            //into gj
                            //from varaus in gj.DefaultIfEmpty()
                                // where-lause  opettaja id tähän?
                            orderby a.alku_aika

                            select new ajatListaData
                            {
                                aika_id = (int)a.aika_id,
                                Alkuaika = (DateTime)a.alku_aika,
                                Kesto = (int)k.kesto,
                                Aihe = a.aihe,
                                Paikka = a.paikka,
                                opettaja_id = (int)a.opettaja_id,
                                Varaaja = v.varaaja_nimi,
                                Varauspvm = (DateTime)v.varattu_pvm,

                            };
            return View(ajatLista);
        }

    }
}
