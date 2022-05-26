using Ajanvarausprojekti.Models;
using Ajanvarausprojekti.Services;
using Ajanvarausprojekti.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class VarausController : Controller
    {
        //Tätä ei saa täältä poistaa, sitä käytetään kaikkialla kontrollerissa
        private aikapalauteEntities db = new aikapalauteEntities();

        //Irina: AikaListaus on kokeiluversio, tarkoitettu pohjaksi, lopullisessa voi poistaa, jos tule käyttöön
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

        //Irina: listaa varaukset, joissa nykyisen open id
        public ActionResult VarausListaus()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Index", "Home");
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
                                    Aihe = v.aihe,
                                    Paikka = a.paikka,
                                    opettaja_id = (int)a.opettaja_id,
                                    Varaaja = v.varaaja_nimi,
                                    Varauspvm = (DateTime)v.varattu_pvm,
                                    varaus_id=v.varaus_id

                                };
                return PartialView("_VarausListaus", ajatLista);
            }


        }



        public ActionResult Index()
        {

            return View();
        }

        //Irina: tällä käyttäjä pystyy poistamaan varaukset
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
                //ONNISTUNUT MODAALI
                //Annetaan tieto varauksen poiston onnistumisesta TempDatalle modaali-ikkunaa varten
                TempData["Successi"] = "Varauksen poisto onnistui!";
                //TempData["BodyText1"] = "";
                //TempData["BodyText2"] = "";
                return RedirectToAction("Index", "Home");
           
            //return RedirectToAction("DeleteConfirmed", "Varaus", varausID); //Tässä määritellään mihin onnistunut toiminto johtaa
            }
            else
            {
                //EionnistunutModaali
                //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Varauksen poisto epäonnistui.";
                TempData["BodyText1"] = "Tarkistathan varauskoodisi.";

                return RedirectToAction("Index", "Home");
               
            }

        }
        // Irina: Tee varauksen (aika_id tulee vielä itse syöttää)
        public ActionResult TeeVaraus()
        {
            ViewBag.aika_id = new SelectList(db.Ajat, "aika_id", "aika_id");

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
                                Aihe = varaus.aihe,
                                Paikka = a.paikka,
                                opettaja_id = (int)a.opettaja_id,
                                Varaaja = varaus.varaaja_sahkoposti,
                                Varauspvm = (DateTime)varaus.varattu_pvm,
                                id_hash = varaus.id_hash


                            };
            return View();
        }

        // Irina: POST: Varaukset/TeeVaraus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeeVaraus([Bind(Include = "aika_id, Varaaja, Aihe, id_hash")] ajatListaData varaus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Tarkistaa onko aika jo varattu                    
                    var testForID = from a in db.Varaukset
                                    where a.aika_id == varaus.aika_id
                                    select a;

                    if (testForID.Any())
                    {
                        ViewBag.Status = "Tämä aika on jo varattu.";
                        return View();
                    }

                    else
                    {
                        //salasana Random
                        LoginService lService = new LoginService();

                        string salasanaRandom = lService.GeneratePassword(3, 3, 3);
                        
                        var testForSalasana = from a in db.Varaukset
                                        where a.id_hash == salasanaRandom
                                        select a;
                        while (testForSalasana.Any())
                        {
                            salasanaRandom = lService.GeneratePassword(3, 3, 3);
                        }
                        
                            //Luodaan varaus
                            Varaukset varauksesi = new Varaukset
                        {
                            varaaja_sahkoposti = varaus.Varaaja,
                            aihe = varaus.Aihe,
                            id_hash = salasanaRandom,
                            aika_id = varaus.aika_id,
                            varattu_pvm = DateTime.Now
                        };
                        db.Varaukset.Add(varauksesi);
                        db.SaveChanges();

                      

                        //Tarkistetaan onko olemassa ja mikä on tän open id  
                        var ajatOpe = (from op in db.Opettajat
                                       join a in db.Ajat on op.opettaja_id equals a.opettaja_id
                                       where varaus.aika_id == a.aika_id
                                       select op).FirstOrDefault();


                        if (ajatOpe != null)
                        {
                            //Tarkistetaan onko olemassa ja mikä on tän open id  
                            var varausAika = (from op in db.Opettajat
                                              join a in db.Ajat on op.opettaja_id equals a.opettaja_id
                                              where varaus.aika_id == a.aika_id
                                              select a).FirstOrDefault();

                            //Irina: sähköpostilähetys
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
                                WebMail.Send(to: ajatOpe.sahkoposti,
                                            subject: "Ohjausaika varattu Tivi-ohjaussovelluksen kautta",
                                            body: "<b><p>Hei!</p></b><br>" +
                                            "<p>Sinulle on tehty ohjausajanvaraus Tivi-ohjaus-sovelluksen kautta ajalle " + varausAika.alku_aika.ToShortDateString() + " " + varausAika.alku_aika.ToShortTimeString() + ". (kesto " + varausAika.kesto_id + " minuuttia.</p><br><p>Paikkana on " + varausAika.paikka + "</p><br><p>Ongelmatilanteissa voit olla yhteydessä sovelluksen pääkäyttäjään Simo Sireniin.</p><br><br>Terveisin, <br> Tivi-ohjaus</p><br>" +
                                            "Tähän viestiin ei voi vastata.", isBodyHtml: true
                                        );
                                ViewBag.Status = "Sähköposti lähetetty. Tarkista sähköpostisi, myös roskapostiviesteistä.";
                                // Send email
                                WebMail.Send(to: varaus.Varaaja,
                                            subject: "Varausvahvistus: Ohjausaika TiVi-opettajalle",
                                            body: "<b><p>Hei!</p></b><br>" +
                                            "Olet tehnyt ohjausajanvarauksen opettajalle Tivi-ohjaus-sovelluksen kautta." + "<p>Varauksen aika " + varausAika.alku_aika + "</p><br>Tapaamisen kesto " + varausAika.kesto_id + " minuuttia. </p><br>" + "</p><br>Teams linkki: " + varausAika.paikka + " </p><br>" +
                                            "<p>Jos haluat perua ajan, voit tehdä sen peruutuskoodin avulla Tivi-ohjaus-sovelluksen kautta.<p><br><p>Peruutuskoodisi:  " + varauksesi.id_hash + "</p><br>Terveisin, <br> Tivi-ohjaus</p><br>" +
                                            "Tähän viestiin ei voi vastata.", isBodyHtml: true
                                        );
                                ViewBag.Status = "Sähköposti lähetetty. Tarkista sähköpostisi, myös roskapostiviesteistä.";
                                @TempData["varausnro"] = varauksesi.id_hash;
                                @TempData["varaaja"] = varaus.Varaaja;
                                @TempData["aihe"] = varaus.Aihe;
                                @TempData["paikka"] = varauksesi.Ajat.paikka;
                                @TempData["aika"] = varausAika.alku_aika;
                                @TempData["kesto"] = varausAika.kesto_id;
                                //return RedirectToAction("OnnistunutVaraus");

                            }
                            catch (Exception)
                            {
                                ViewBag.Status = "Et ole antanut sähköpostiosoitetta.";

                            }


                        }
                        else
                        {
                            ViewBag.Status = "Opettajaa ei löytynyt";
                        }
                    }
                 //ONNISTUNUT MODAALI
                //Annetaan tieto varauksen onnistumisesta TempDatalle modaali-ikkunaa varten
                //TempData["Successi"] = "Varaus onnistui!";
                //TempData["BodyText1"] = "Saat pian antamaasi sähköpostiosoitteeseen varausvahvistuksen, jos annoit sähköpostiosoitteesi..";
                //TempData["BodyText2"] = "Voit tarvittaessa perua varauksen sähköpostissa olevalla peruutuskoodilla.";
                //    //jos ope tallennus onnistuu lähettää userin tähän

                    //jos ope tallennus onnistuu lähettää userin tähän
                    return RedirectToAction("OnnistunutVaraus");


                }

                //Tähän mennee, jos model ei ole validi

                //EionnistunutModaali
                    //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                    TempData["BodyText1"] = "Varauksen lähetys epäonnistui.";
                    
                return RedirectToAction("Index", "Home");

            }
            catch
            {
                //EPAONNISTUNUT MODAALI
                //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Hups! Jokin meni nyt pieleen!";
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult OnnistunutVaraus()
        {
            return View();
        }

        //Irina:yksittäisen varauksen tarkempi kuvaus
        // GET: Varaukset/Details/5
        public ActionResult _VarausListModal(int? id)
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
            return PartialView("_VarausListModal",varaus);
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
