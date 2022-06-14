using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.Services;
using Ajanvarausprojekti.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class SuperUserController : Controller
    {
        //Tätä ei saa täältä poistaa, sitä käytetään kaikkialla kontrollerissa
        private aikapalauteEntities db = new aikapalauteEntities();

        //Yhteystiedot-olio
        private Yhteystiedot ohjelmanyhteystiedot = new Yhteystiedot();

        // GET: SuperUser
        public ActionResult Index()
        {
            return View();
        }
        // Irina: Listaa opettajat
        public ActionResult _ListaaOpettajat()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                List<Opettajat> model = db.Opettajat.ToList();
                return PartialView("_ListaaOpettajat", model);
            }
        }
        //Irina: Lisaa uuden opettajan ja tälle käyttäjätunnuksen ja oikeudet, basicuser defaultina
        public ActionResult LisaaOpettaja()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {


                ViewBag.oikeudet_id = new SelectList(db.Yllapitooikeudet, "oikeudet_id", "oikeudet");

                var createOpe = from o in db.Opettajat
                                join k in db.Kayttajatunnukset on o.opettaja_id equals k.opettaja_id

                                select new UusiOpe
                                {
                                    opettaja_id = (int)o.opettaja_id,
                                    kuva = o.kuva,
                                    etunimi = o.etunimi,
                                    sukunimi = o.sukunimi,
                                    nimike = o.nimike,
                                    sahkoposti = o.sahkoposti,
                                    kayttajatunnus = k.kayttajatunnus,
                                    salasana = k.salasana,
                                    oikeudet_id = k.oikeudet_id

                                };
            }
            return View();
        }
        //Irina: POST: Lisaa uuden opettajan ja tälle käyttäjätunnuksen, oikeudet ja kuva Opekuvat kansioon. Basicuser defaultina
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LisaaOpettaja(UusiOpe uusiope, HttpPostedFileBase file)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
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
                            //Jos file null niin tämä kaatuu
                            //if (file.ContentLength > 0)

                            //Jos tiedosto on ladattu
                            if (file != null)
                            {
                                if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                                {
                                    string _FileName = Path.GetFileName(file.FileName);
                                    string _path = Path.Combine(Server.MapPath("~/Opekuvat"), _FileName);
                                    file.SaveAs(_path);

                                    //Luodaan ope
                                    string _FileName1 = Path.GetFileName(file.FileName);
                                    Opettajat luoopettaja = new Opettajat
                                    {
                                        sahkoposti = uusiope.sahkoposti,
                                        etunimi = uusiope.etunimi,
                                        sukunimi = uusiope.sukunimi,
                                        nimike = uusiope.nimike,
                                        kuva = "/Opekuvat/" + _FileName1
                                    };
                                    db.Opettajat.Add(luoopettaja);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    ViewBag.Message = "Valitsehan jpeg,jpg tai png tiedoston.";
                                    return View();
                                }

                            }
                            //jos tiedosto ei ole ladattu, käytetään default kuvaa
                            else
                            {
                                //opekuva defalt polku, tällä hetkellä "/Opekuvat/defaultKuva.jpg"

                                string opekuvadefault = ohjelmanyhteystiedot.OpeDefaultKuva;

                                Opettajat luoopettaja = new Opettajat
                                {
                                    sahkoposti = uusiope.sahkoposti,
                                    etunimi = uusiope.etunimi,
                                    sukunimi = uusiope.sukunimi,
                                    nimike = uusiope.nimike,
                                    kuva = opekuvadefault
                                };
                                db.Opettajat.Add(luoopettaja);
                                db.SaveChanges();

                            }



                            LoginService lService = new LoginService();
                            string kryptattuSalasana = lService.md5_string(uusiope.salasana);

                            //Tarkistetaan onko olemassa ja mikä on tän open id
                            Opettajat opekayttis = new Opettajat();
                            opekayttis = (from o in db.Opettajat
                                          where uusiope.etunimi == o.etunimi && uusiope.sukunimi == o.sukunimi && uusiope.sahkoposti == o.sahkoposti
                                          select o).FirstOrDefault();

                            if (opekayttis != null)
                            {
                                //luodaan käyttäjätunnuksen opelle
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
                                        Yhteystiedot ohjelmanyhteystiedot = new Yhteystiedot();
                                        string sahkopostiosoite_ohjelman = ohjelmanyhteystiedot.OhjelmanSahkopostiosoite;
                                        string spostisalasana = ohjelmanyhteystiedot.OhjelmanSpostiSalasana;
                                        //Configuring webMail class to send emails  
                                        //gmail smtp server  
                                        WebMail.SmtpServer = "smtp.gmail.com";
                                        //gmail port to send emails  
                                        WebMail.SmtpPort = 587;
                                        WebMail.SmtpUseDefaultCredentials = true;
                                        //sending emails with secure protocol  
                                        WebMail.EnableSsl = true;
                                        //EmailId used to send emails from application  
                                        WebMail.UserName = sahkopostiosoite_ohjelman;
                                        WebMail.Password = spostisalasana;

                                        //Sender email address.  
                                        WebMail.From = sahkopostiosoite_ohjelman;

                                        // Send email
                                        WebMail.Send(to: opettajasposti.sahkoposti,
                                                    subject: "Salasana ja käyttäjätunnus luotu TiVi-sivustolle",
                                                    body: "<b><p>Hei!</p></b><br>" +
                                                    "Salasana ja käyttäjätunnus luotu TiVi-sivustolla. Mikäli olet tilannut salasanan vaihtoa, voit jättää tämän viestin huomiotta." +
                                                    "<p>Ole yhteydessä TiVi-sivuston pääkäyttäjään </p><br><br>Terveisin, <br> Tivi</p><br> +" +
                                                    "Tähän viestiin ei voi vastata.", isBodyHtml: true
                                                );
                                        //ViewBag.Status = "Sähköposti lähetetty. Tarkista sähköpostisi, myös roskapostiviesteistä.";
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

                    //ONNISTUNUT Ilmoitus
                   
                    TempData["Success"] = "Käyttäjän lisäys onnistui!";
                    //TempData["BodyText1"] = "Lisäämäsi opettaja saa pian antamaasi sähköpostiosoitteeseen varausvahvistuksen.";
                    //TempData["BodyText2"] = "";
                    //jos ope tallennus onnistuu lähettää userin tähän
                    return RedirectToAction("LisaaOpettaja");
                }
                catch
                {
                    //EionnistunutModaali
                    //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Käyttäjän lisäys epäonnistui.";
                    //TempData["BodyText1"] = "Opettajan lisäys epäonnistui.";
                    ViewBag.Status = "Syöttämissäsi tiedoissa jo jokin virhe.";
                    return View();
                }
            }
        }

        // Irina: Poistetaan opettaja
        public ActionResult _PoistaOpettaja(int? id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                Opettajat opettaja = db.Opettajat.Find(id);
                if (opettaja == null) return HttpNotFound();
                return PartialView("_PoistaOpettaja", opettaja);
            }
        }

        // Irina: POST: Opettaja/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _PoistaOpettaja(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Opettajat opettaja = db.Opettajat.Find(id);


                //Poistetaan myös tiedostoista opettajan kuvan, jos ei ole default kuva
                //opekuva defalt polku, tällä hetkellä "/Opekuvat/defaultKuva.jpg"

                string opekuvadefault = ohjelmanyhteystiedot.OpeDefaultKuva;
                if (opettaja.kuva != opekuvadefault)
                {
                    //opettajan kuvan poisto
                    string _FileName = opettaja.kuva;

                    //string fullPath = Path.Combine(Server.MapPath("~"), _FileName);
                    string fullPath = _FileName;
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                db.Opettajat.Remove(opettaja);
                db.SaveChanges();
                //ONNISTUNUT Ilmoitus

                TempData["Success"] = "Käyttäjän poisto onnistui!";
                return RedirectToAction("LisaaOpettaja");
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

