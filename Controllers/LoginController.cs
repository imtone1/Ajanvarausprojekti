using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ajanvarausprojekti.Functions;
using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.Services;

namespace Ajanvarausprojekti.Controllers
{
    public class LoginController : Controller
    {

        private aikapalauteEntities db = new aikapalauteEntities();


        public ActionResult Create()
        {
            ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi");
            ViewBag.oikeudet_id = new SelectList(db.Yllapitooikeudet, "oikeudet_id", "oikeudet");


            return View();
        }

        // POST: Create
      [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "kayttajatunnus, salasana, opettaja_id, oikeudet_id")] Kayttajatunnukset kayttaja)
        {

            LoginService lService = new LoginService();

            if (ModelState.IsValid)
            {
                var testForUser = from k in db.Kayttajatunnukset
                            where k.kayttajatunnus == kayttaja.kayttajatunnus
                            select k;

                
                if (testForUser.Any())
                {
                    ViewBag.Kayttajaolemassa = "Käyttäjää ei lisätty! Kyseinen käyttäjätunnus on jo olemassa järjestelmässä.";
                }

                else
                {
                    
                    string kryptattuSalasana = lService.md5_string(kayttaja.salasana);

                   kayttaja = new Kayttajatunnukset
                    {
                        kayttajatunnus = kayttaja.kayttajatunnus,
                        opettaja_id = kayttaja.opettaja_id,
                        oikeudet_id = kayttaja.oikeudet_id,
                        salasana = kryptattuSalasana,
                    };

                    db.Kayttajatunnukset.Add(kayttaja);
                    db.SaveChanges();

                    //tämä itapu sovelluksesta, näyttäisi liittyvän sähköpostin lähettämiseen
                   
                    //string hash = kayttaja.salasana;

                    //string hyperlink = "<a href=\"" + "https://localhost:44388/login/NewPassword/?email=" + kayttaja.Sahkoposti + "&hash=" + hash + "\">Luo uusi salasana</a>";
                    //var body = "<b><p>Hei,</p></b><br>" +
                    //    "Sinulle on luotu tunnukset Careerian Digihelpissä! Seuraavaksi sinun tulee luoda itsellesi uusi salasana." +
                    //    "<p>Siirry sivustolle: " + hyperlink + "</p><br><br>Terveisin, <br> Digihelppi</p><br> +" +
                    //    "Tähän viestiin ei voi vastata.";
                    //var message = new MailMessage();
                    //message.To.Add(new MailAddress(kayttajat.Sahkoposti));
                    //message.Subject = "Salasanan uusiminen";
                    //message.Body = string.Format(body);
                    //message.IsBodyHtml = true;

                    //using (var smtp = new SmtpClient())
                    //{
                    //    await smtp.SendMailAsync(message);
                    //    return RedirectToAction("Index");
                    //}
                }
            }

            
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(Kayttajatunnukset LoginModel)
        {
            //salasanan hash
            var crpwd = "";
            var salt = Hmac.GenerateSalt();
            var hmac1 = Hmac.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(LoginModel.salasana), salt);
            crpwd = (Convert.ToBase64String(hmac1));

            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Kayttajatunnukset.SingleOrDefault(x => x.kayttajatunnus == LoginModel.kayttajatunnus && x.salasana == crpwd);
             
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


        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        //// GET: Login/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: Login/Create
        //public ActionResult Create()
        //{
        //    ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi");
        //    ViewBag.oikeudet_id = new SelectList(db.Yllapitooikeudet, "oikeudet_id", "oikeudet");
        //    return View();
        //}

        //// POST: Login/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "kayttajatunnus, salasana, opettaja_id, oikeudet_id")] Kayttajatunnukset kayttaja)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //var crpwd = "";
        //            //var salt = Hmac.GenerateSalt();
        //            //var hmac1 = Hmac.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(kayttaja.salasana), salt);
        //            //crpwd = (Convert.ToBase64String(hmac1));


        //            db.Kayttajatunnukset.Add(kayttaja);
        //            db.SaveChanges();
        //            return RedirectToAction("Login", "Login");
        //        }

        //        ViewBag.opettaja_id = new SelectList(db.Opettajat, "opettaja_id", "etunimi", "sukunimi", kayttaja.opettaja_id);
        //        ViewBag.oikeudet_id = new SelectList(db.Yllapitooikeudet, "oikeudet_id", "oikeudet", kayttaja.oikeudet_id);


        //        return View(kayttaja);



        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", "Home");

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
