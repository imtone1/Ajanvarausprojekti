using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.Services;
using Ajanvarausprojekti.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class SuperUserController : Controller
    {
        //Tätä ei saa täältä poistaa, sitä käytetään kaikkialla kontrollerissa
        private aikapalauteEntities db = new aikapalauteEntities();
        // GET: SuperUser
        public ActionResult Index()
        {
            return View();
        }

        //Irina: Lisaa uuden opettajan ja tälle käyttäjätunnuksen ja oikeudet, basicuser defaultina
        public ActionResult LisaaOpettaja()
        {


            ViewBag.oikeudet_id = new SelectList(db.Yllapitooikeudet, "oikeudet_id", "oikeudet");

            var createOpe = from o in db.Opettajat
                            join k in db.Kayttajatunnukset on o.opettaja_id equals k.opettaja_id

                            select new UusiOpe
                            {
                                opettaja_id = (int)o.opettaja_id,
                                etunimi = o.etunimi,
                                sukunimi = o.sukunimi,
                                nimike = o.nimike,
                                sahkoposti = o.sahkoposti,
                                kayttajatunnus = k.kayttajatunnus,
                                salasana = k.salasana,
                                oikeudet_id = k.oikeudet_id,

                            };
            return View();
        }
        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LisaaOpettaja([Bind(Include = "kayttajatunnus, salasana, oikeudet_id, sahkoposti, etunimi, sukunimi, nimike")] UusiOpe uusiope)
        {
            //mietinnässä vielä kannattaako tätä try catch jakaa pienempiin osiin
            try
            {


                if (ModelState.IsValid)
                {
                    //Tarkistaa onko käyttäjätunnus jo olemassa
                    var testForUser = from k in db.Kayttajatunnukset
                                      where k.kayttajatunnus == uusiope.kayttajatunnus
                                      select k;
                    if (testForUser.Any())
                    {
                        ViewBag.Kayttajaolemassa = "Kyseinen käyttäjätunnus on jo olemassa järjestelmässä.";
                        return View();
                    }

                    else
                    {
                        //Luodaan ope
                        Opettajat luoopettaja = new Opettajat
                        {
                            sahkoposti = uusiope.sahkoposti,
                            etunimi = uusiope.etunimi,
                            sukunimi = uusiope.sukunimi,
                            nimike = uusiope.nimike
                        };

                        db.Opettajat.Add(luoopettaja);
                        db.SaveChanges();

                        LoginService lService = new LoginService();
                        string kryptattuSalasana = lService.md5_string(uusiope.salasana);

                        //Tarkistetaan onko olemassa ja mikä on tän open id
                        Opettajat opekayttis = new Opettajat();
                        opekayttis = (from o in db.Opettajat
                                      where uusiope.etunimi == o.etunimi && uusiope.sukunimi == o.sukunimi && uusiope.sahkoposti == o.sahkoposti
                                      select o).FirstOrDefault();

                        if (opekayttis != null)
                        {

                            Kayttajatunnukset kayttis = new Kayttajatunnukset
                            {
                                kayttajatunnus = uusiope.kayttajatunnus,
                                opettaja_id = opekayttis.opettaja_id,
                                oikeudet_id = uusiope.oikeudet_id,
                                salasana = kryptattuSalasana,
                            };

                            db.Kayttajatunnukset.Add(kayttis);
                            db.SaveChanges();

                            //sähköpostilähetys

                            Opettajat opettajasposti = new Opettajat();

                            opettajasposti = (from o in db.Opettajat
                                              join k in db.Kayttajatunnukset on o.opettaja_id equals k.opettaja_id
                                              where kayttis.opettaja_id == o.opettaja_id
                                              select o).FirstOrDefault();

                            if (opettajasposti != null)
                            {

                                try
                                {
                                    //Configuring webMail class to send emails  
                                    //gmail smtp server  
                                    WebMail.SmtpServer = "smtp.gmail.com";
                                    //gmail port to send emails  
                                    WebMail.SmtpPort = 587;
                                    WebMail.SmtpUseDefaultCredentials = true;
                                    //sending emails with secure protocol  
                                    WebMail.EnableSsl = true;
                                    //EmailId used to send emails from application  
                                    WebMail.UserName = "tivisovellus@gmail.com";
                                    WebMail.Password = "1hAn5!VAiO1k9";

                                    //Sender email address.  
                                    WebMail.From = "tivisovellus@gmail.com";

                                    // Send email
                                    WebMail.Send(to: opettajasposti.sahkoposti,
                                                subject: "Salasana ja käyttäjätunnus luotu TiVi-sivustolle",
                                                body: "<b><p>Hei!</p></b><br>" +
                                                "Salasana ja käyttäjätunnus luotu TiVi-sivustolla. Mikäli olet tilannut salasanan vaihtoa, voit jättää tämän viestin huomiotta." +
                                                "<p>Ole yhteydessä TiVi-sivuston pääkäyttäjään </p><br><br>Terveisin, <br> Tivi</p><br> +" +
                                                "Tähän viestiin ei voi vastata.", isBodyHtml: true
                                            );
                                    ViewBag.Status = "Sähköposti lähetetty. Tarkista sähköpostisi, myös roskapostiviesteistä.";
                                }
                                catch (Exception)
                                {
                                    ViewBag.Status = "Et ole antanut sähköpostiosoitteen.";

                                }
                            }
                            else
                            {
                                ViewBag.Status = "Opettajaa ei löytynyt";
                            }
                        }
                    }

                }
                //jos ope tallennus onnistuu lähettää userin tähän
                return RedirectToAction("Index", "Home");
            }
            catch
            {

                ViewBag.Status = "Syöttämissäsi tiedoissa jo jokin virhe.";
                return View();
            }

        }

        //Dispose pakko olla, tätä ei saa poistaa!
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Alla oleva koodi generoitunut automaattisesti, lopullisessa versiossa poistetaan, jos ei ole käyttöä.
        // GET: SuperUser/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: SuperUser/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: SuperUser/Create
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

        //// GET: SuperUser/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: SuperUser/Edit/5
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

        //// GET: SuperUser/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: SuperUser/Delete/5
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