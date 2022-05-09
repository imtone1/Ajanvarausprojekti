using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class VarausController : Controller
    {
        private aikapalauteEntities db = new aikapalauteEntities();

        public ActionResult AikaListaus()
        {
            //left join, jotta näkyisi kaikki ajat, myös ne joissa ei varausta
            var ajatLista = from a in db.Ajat
                            join op in db.Opettajat on a.opettaja_id equals op.opettaja_id
                            join k in db.Kestot on a.kesto_id equals k.kesto_id
                            join v in db.Varaukset on a.aika_id equals v.aika_id
                            into gj
                            from varaus in gj.DefaultIfEmpty()
                                // where-lause  opettaja id tähän jos halutaan, että tietyn opettajan ajat näkyisivät
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
            return PartialView("_AikaListaus", ajatLista);
        }

        public ActionResult VarausListaus()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                //Sessiosta otetaan kirjautuneen opettajan id
                var opeid = Session["OpettajaID"];
                int opeOikID = int.Parse(opeid.ToString());
                //Näkyy kirjautuneen opettajan ajat sekä tämänpäiväset tai tulevat ajat
                var ajatLista = from a in db.Ajat
                                join op in db.Opettajat on a.opettaja_id equals op.opettaja_id
                                join k in db.Kestot on a.kesto_id equals k.kesto_id
                                join v in db.Varaukset on a.aika_id equals v.aika_id
                                where op.opettaja_id == opeOikID && a.alku_aika >= DateTime.Today
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
                return PartialView("_VarausListaus", ajatLista);
            }


        }

        //Dispose pakko olla, sitä ei saa poistaa
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        //Alla olevaa koodia voi käyttää pohjana tai olla käyttämättä kokonaan. Saa poistaa, jos ei tarvetta.
        // GET: Ajat

        public ActionResult Index()
        {

            return View();
        }

        //// GET: Ajat/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ajat ajat = db.Ajat.Find(id);
        //    if (ajat == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ajat);
        //}

        //// GET: Ajat/Create
        //public ActionResult Create()
        //{
        //    ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id");
        //    ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti");
        //    return View();
        //}

        //// POST: Ajat/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id,aihe,paikka")] Ajat ajat)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Ajat.Add(ajat);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
        //    ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
        //    return View(ajat);
        //}

        //// GET: Ajat/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ajat ajat = db.Ajat.Find(id);
        //    if (ajat == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
        //    ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
        //    return View(ajat);
        //}

        //// POST: Ajat/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "aika_id,alku_aika,kesto_id,opettaja_id,aihe,paikka")] Ajat ajat)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ajat).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.kesto_id = new SelectList(db.Kestot, "kesto_id", "kesto_id", ajat.kesto_id);
        //    ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "sahkoposti", ajat.opettaja_id);
        //    return View(ajat);
        //}

        public ActionResult VarausPoisto()
        {
            //var varaus = db.Varaukset;

            return PartialView();
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Varaukset varaus = db.Varaukset.Find(id);
            if (varaus == null)
            {
                return HttpNotFound();
            }
            return View(varaus);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult VarausPoisto(Varaukset varaus)
        {
            var varaaja = db.Varaukset.SingleOrDefault(x => x.id_hash == varaus.id_hash);
            
            if (varaaja != null)
            {
                
                int varausID = (from v in db.Varaukset
                                    where v.id_hash== varaaja.id_hash
                                    select v.varaus_id).Take(1).SingleOrDefault();
               
                int varaus_id = varaaja.varaus_id;

                Varaukset varauspoisto = db.Varaukset.Find(varausID);
                db.Varaukset.Remove(varauspoisto);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
           
            //return RedirectToAction("DeleteConfirmed", "Varaus", varausID); //Tässä määritellään mihin onnistunut toiminto johtaa
            }
            else
            {
                return View("Error");
            }

        }

       
        // GET: Ajat/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Varaukset varaus = db.Varaukset.Find(id);
        //    if (varaus == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(varaus);
        //}

        // POST: Ajat/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Varaukset varaus = db.Varaukset.Find(id);
        //    db.Varaukset.Remove(varaus);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


    }
}
