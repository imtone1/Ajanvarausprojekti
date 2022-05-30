using Ajanvarausprojekti.Functions;
using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class LoginController : Controller
    {

        private aikapalauteEntities db = new aikapalauteEntities();
        
        
        // Irina:Kirjautuminen ja sessioiden luominen
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(Kayttajatunnukset LoginModel)
        {
            try
            {
                //salasanan hash
                var crpwd = "";
                var salt = Hmac.GenerateSalt();
                var hmac1 = Hmac.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(LoginModel.salasana), salt);
                crpwd = (Convert.ToBase64String(hmac1));

                //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
                var LoggedUser = db.Kayttajatunnukset.SingleOrDefault(x => x.kayttajatunnus == LoginModel.kayttajatunnus && x.salasana == crpwd);

                if (LoggedUser != null)
                {

                    Session["UserName"] = LoggedUser.kayttajatunnus;
                    Session["LoginID"] = LoggedUser.kayttajatunnus_id;
                    Session["Opettaja"] = LoggedUser.Opettajat.etunimi.ToString();
                    Session["KirjautunutOpe"] = LoggedUser.Opettajat.etunimi.ToString() + " " + LoggedUser.Opettajat.sukunimi.ToString(); ;
                    Session["OpettajaID"] = LoggedUser.Opettajat.opettaja_id;
                    //Session["AccessLevel"] = LoggedUser.oikeudet_id;

                    //tarkistetaan oikeudet 1 on superuser
                    if (LoggedUser.oikeudet_id == 1)
                    {
                        Session["Admin"] = LoggedUser.kayttajatunnus;
                    }

                    ViewBag.LoginMessage = "Successfull login";
                    ViewBag.LoggedStatus = "In";
                    ViewBag.LoginError = 0;

                    return RedirectToAction("OpettajienSivu", "Home"); //Tässä määritellään mihin onnistunut kirjautuminen johtaa


                }
                else
                {
                    //ViewBag.LoginMessage = "Login unsuccessfull";
                    //ViewBag.LoggedStatus = "Out";
                    //ViewBag.LoginError = 1;
                    //LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                    //EionnistunutModaali
                    //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Kirjautuminen epäonnistui!";
                    TempData["BodyText1"] = "Tarkista käyttäjätunnus ja salasana.";

                    return RedirectToAction("Index", "Home");

                    //return View("Login", LoginModel);
                }
            }
            catch
            {
                TempData["Errori"] = "Kirjautuminen epäonnistui!";
                TempData["BodyText1"] = "Tarkista käyttäjätunnus ja salasana.";

                return RedirectToAction("Index", "Home");
            }
        }

        //Irina: logout toiminto
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Out";
            return RedirectToAction("Index", "Home"); //Uloskirjautumisen jälkeen pääsivulle
        }

        //tämä create toimintoa jo käytetään SuperUserControllerissa LisaaOpettaja toiminnossa. Lopullisessa versiossa poistetaan tämä create, jos ei käyttöä. Nyt testausmielessä kommentoitu

        //public ActionResult Create()
        //{
        //    ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi");
        //    ViewBag.oikeudet_id = new SelectList(db.Yllapitooikeudet, "oikeudet_id", "oikeudet");


        //    return View();
        //}

        //// POST: Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "kayttajatunnus, salasana, opettaja_id, oikeudet_id")] Kayttajatunnukset kayttaja)
        //{
        //    try
        //    {
        //        LoginService lService = new LoginService();

        //        if (ModelState.IsValid)
        //        {
        //            var testForUser = from k in db.Kayttajatunnukset
        //                              where k.kayttajatunnus == kayttaja.kayttajatunnus
        //                              select k;


        //            if (testForUser.Any())
        //            {
        //                ViewBag.Kayttajaolemassa = "Käyttäjää ei lisätty! Kyseinen käyttäjätunnus on jo olemassa järjestelmässä.";
        //            }

        //            else
        //            {

        //                string kryptattuSalasana = lService.md5_string(kayttaja.salasana);

        //                kayttaja = new Kayttajatunnukset
        //                {
        //                    kayttajatunnus = kayttaja.kayttajatunnus,
        //                    opettaja_id = kayttaja.opettaja_id,
        //                    oikeudet_id = kayttaja.oikeudet_id,
        //                    salasana = kryptattuSalasana,
        //                };

        //                db.Kayttajatunnukset.Add(kayttaja);
        //                db.SaveChanges();


        //                //jemmaan tämän >ei vielä käytetä missään
        //                //var opettaja = from o in db.Opettajat
        //                //                  join k in db.Kayttajatunnukset on o.opettaja_id equals k.opettaja_id

        //                //                  where k.kayttajatunnus_id==LoggedUser.kayttajatunnus_id
        //                //                  //orderby-lause
        //                //                  select new Opettajat
        //                //                  {
        //                //                      etunimi = o.etunimi,
        //                //                  }; 

        //                //sähköpostilähetys

        //                Opettajat opettajasposti = new Opettajat();

        //                opettajasposti = (from o in db.Opettajat
        //                                  join k in db.Kayttajatunnukset on o.opettaja_id equals k.opettaja_id
        //                                  where kayttaja.opettaja_id == o.opettaja_id
        //                                  select o).FirstOrDefault();

        //                if (opettajasposti != null)
        //                {


        //                    //ModelState.AddModelError("", "Sähköpostiosoitetta ei löytynyyt.");

        //                    try
        //                    {
        //                        //Configuring webMail class to send emails  
        //                        //gmail smtp server  
        //                        WebMail.SmtpServer = "smtp.gmail.com";
        //                        //gmail port to send emails  
        //                        WebMail.SmtpPort = 587;
        //                        WebMail.SmtpUseDefaultCredentials = true;
        //                        //sending emails with secure protocol  
        //                        WebMail.EnableSsl = true;
        //                        //EmailId used to send emails from application  
        //                        WebMail.UserName = "tivisovellus@gmail.com";
        //                        WebMail.Password = "1hAn5!VAiO1k9";

        //                        //Sender email address.  
        //                        WebMail.From = "tivisovellus@gmail.com";

        //                        //Send email  

        //                        // Send email
        //                        WebMail.Send(to: opettajasposti.sahkoposti,
        //                                    subject: "Salasana ja käyttäjätunnus luotu TiVi-sivustolle",
        //                                    body: "<b><p>Hei!</p></b><br>" +
        //                                    "Salasana ja käyttäjätunnus luotu TiVi-sivustolla. Mikäli olet tilannut salasanan vaihtoa, voit jättää tämän viestin huomiotta." +
        //                                    "<p>Ole yhteydessä TiVi-sivuston pääkäyttäjään </p><br><br>Terveisin, <br> Tivi</p><br> +" +
        //                                    "Tähän viestiin ei voi vastata.", isBodyHtml: true
        //                                );
        //                        ViewBag.Status = "Sähköposti lähetetty. Tarkista sähköpostisi, myös roskapostiviesteistä.";
        //                    }
        //                    catch (Exception)
        //                    {
        //                        ViewBag.Status = "Et ole antanut sähköpostiosoitteen.";

        //                    }
        //                }
        //                else
        //                {
        //                    ViewBag.Status = "Opettajaa ei löytynyt";
        //                }
        //            }
        //        }


        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch
        //    {

        //        return View();
        //    }
        //}

        //Irina: Saa käyttää tai jos varausten create on tehty niin saa poistaa. Tämä liittyy varauksen luomiseen, kesken, koska testataakseen salasanan generoimisen olisi pitänyt tehdä kokonaan varausten luomisen.
        //public ActionResult GeneroiSalasana()
        //{

        //    return View();

        //}

        //// POST: 
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GeneroiSalasana(Varaukset varaus)
        //{
        //    try
        //    {
        //        LoginService lServicee = new LoginService();
        //        string varausSalasana = lServicee.GeneratePassword(3, 3, 3);

        //        if (ModelState.IsValid)
        //        {
        //            var testForPass = from v in db.Varaukset
        //                              where k.kayttajatunnus == kayttaja.kayttajatunnus
        //                              select k;



        //public ActionResult Index()
        //{
        //    return View();
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
