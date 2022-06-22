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
        //Yhteystiedot-olio
        private Yhteystiedot ohjelmanyhteystiedot = new Yhteystiedot();

        // Irina:Kirjautuminen ja sessioiden luominen
        public ActionResult Login()
        {
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
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

                    Session["OpettajaKuva"] = LoggedUser.Opettajat.kuva;

                    //Session["AccessLevel"] = LoggedUser.oikeudet_id;

                    //tarkistetaan oikeudet 1 on superuser
                    if (LoggedUser.oikeudet_id == 1)
                    {
                        Session["Admin"] = LoggedUser.kayttajatunnus;
                    }

                    ViewBag.LoginMessage = "Successfull login";
                    ViewBag.LoggedStatus = "In";
                    ViewBag.LoginError = 0;
                    //poistoajan määrittely, tällä hetkellä 2pv
                    Yhteystiedot ohjelmanyhteystiedot = new Yhteystiedot();
                    int poistointervalli= ohjelmanyhteystiedot.PoistoIntervalli;

                    //Irina: vanhemmat kuin 2 pv ajat poisto järjestelmästä
                    DateTime neljatoista = DateTime.Today.AddDays(poistointervalli);


                    var aikaolemassa = from a in db.Ajat
                                       where a.alku_aika < neljatoista
                                       select a;

                    while (aikaolemassa.Any())
                    {
                        int aikaID = (from a in db.Ajat
                                      where a.alku_aika < neljatoista
                                      select a.aika_id).Take(1).SingleOrDefault();

                        Ajat aikapoisto = db.Ajat.Find(aikaID);
                        db.Ajat.Remove(aikapoisto);

                        db.SaveChanges();

                    }

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

    }
}
