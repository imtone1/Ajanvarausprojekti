using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajanvarausprojekti.Models;

namespace Ajanvarausprojekti.Controllers
{
    public class LoginController : Controller
    {

        private aikapalauteEntities db = new aikapalauteEntities();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(Kayttajatunnukset LoginModel)
        {
            //luodaan db olio
            

          
            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Kayttajatunnukset.SingleOrDefault(x => x.kayttajatunnus == LoginModel.kayttajatunnus && x.salasana == LoginModel.salasana);
             
            //jemmaan tämän >ei vielä käytetä missään
            //var opettaja = from o in db.Opettajat
             //                  join k in db.Kayttajatunnukset on o.opettaja_id equals k.opettaja_id
                              
             //                  where k.kayttajatunnus_id==LoggedUser.kayttajatunnus_id
             //                  //orderby-lause
             //                  select new Opettajat
             //                  {
             //                      etunimi = o.etunimi,
             //                  }; 
            
            if (LoggedUser != null)
            {
                //tarkistetaan oikeudet 1 on superuser
                if (LoggedUser.oikeudet_id == 1) {
                ViewBag.LoginMessage = "Successfull login";
                ViewBag.LoggedStatus = "In";
                ViewBag.LoginError = 0;
                Session["UserName"] = LoggedUser.kayttajatunnus; 
                Session["LoginID"] = LoggedUser.kayttajatunnus_id;
                //Session["Opettaja"] = LoggedUser.Opettajat.etunimi.ToString();
                //Session["AccessLevel"] = LoggedUser.oikeudet_id;
                    return RedirectToAction("Index", "Ajanvaraus"); //Tässä määritellään mihin onnistunut kirjautuminen johtaa --> Home/Index
                 }

                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                ViewBag.LoginError = 1;
                LoginModel.LoginErrorMessage = "Käyttäjäoikeudet basicuser.";
                return View("Login", LoginModel);
            }
            else
            {
                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                ViewBag.LoginError = 1;
                LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Login", LoginModel);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Out";
            return RedirectToAction("Index", "Home"); //Uloskirjautumisen jälkeen pääsivulle
        }

        
        //// GET: Login
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: Login/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Login/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Login/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Login/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Login/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Login/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Login/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
