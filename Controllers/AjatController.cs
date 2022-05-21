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