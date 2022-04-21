using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajanvarausprojekti.Models;


namespace Ajanvarausprojekti.Controllers
{
    public class AjanvarausController : Controller
    {
        private aikapalauteEntities db = new aikapalauteEntities();
        // GET: Ajanvaraus
        public ActionResult Index()
        {
            //luodaan db olio

            List<Ajat> model = db.Ajat.ToList();

            //poistetaan/vapautetaan olio db, koska muuten luodaan liian monta oliota/tietokantayhteyksia
            db.Dispose();

            return View(model);
        }

       
        // GET: Ajanvaraus/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Ajanvaraus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ajanvaraus/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ajanvaraus/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Ajanvaraus/Edit/5
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

        // GET: Ajanvaraus/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ajanvaraus/Delete/5
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
    }
}
