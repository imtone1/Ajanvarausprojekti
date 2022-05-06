using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajanvarausprojekti.Models;

namespace Ajanvarausprojekti.Controllers
{
    public class PalauteController : Controller
    {
        //Huom! tietokantayhteys voidaan luoda täällä ylätasollakin jolloin se näkyy jokaiseen metodiin.
        //Mutta muistetaan vapauttaa olio sitten disposella aina kun sitä on käytetty.

        // GET: Palaute
        public ActionResult Index()
        {
            //luodaan db olio
            aikapalauteEntities db = new aikapalauteEntities();

            //Listataan kaikki palautteet Palaute-näkymän Index-sivulle (Vews-Palaute-Index)
            List<Palautteet> model = db.Palautteet.ToList();

            //poistetaan/vapautetaan olio db, koska muuten luodaan liian monta oliota/tietokantayhteyksia
            db.Dispose();

            return View(model);
        }

        public ActionResult _Index()
        {
            //luodaan db olio
            aikapalauteEntities db = new aikapalauteEntities();

            //Listataan kaikki palautteet Palaute-näkymän Index-sivulle (Vews-Palaute-Index)
            List<Palautteet> model = db.Palautteet.ToList();

            //poistetaan/vapautetaan olio db, koska muuten luodaan liian monta oliota/tietokantayhteyksia
            db.Dispose();

            return PartialView("_Index", model);
        }

        // GET: Palaute/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Palaute/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Palaute/Create
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
    }
}
