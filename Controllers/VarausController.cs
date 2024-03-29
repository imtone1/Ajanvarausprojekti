﻿using Ajanvarausprojekti.Models;
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

        //Yhteystiedot-olio
        private Yhteystiedot ohjelmanyhteystiedot = new Yhteystiedot();

        //Irina: AikaListaus tarkoitettu pohjaksi
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
                                    Varaaja = v.varaaja_sahkoposti,
                                    Varauspvm = (DateTime)v.varattu_pvm,
                                    varaus_id=v.varaus_id

                                };

                if (ajatLista.Count() > 0)
                {
                    ViewBag.EiVarauksia = "Sinulla ei ole varauksia.";
                }
                else
                {
                    ViewBag.EiVarauksia = "";
                }
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
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
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
        [ValidateAntiForgeryToken]
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

        // Tämä antaa valitun ajan id:n varaukselle.

        public ActionResult TeeVaraus(int? id)
        {
            ViewBag.aika_id = id;

            //Tarkistetaan onko olemassa ja mikä on tän open id  
            var ope= (from op in db.Opettajat
                      join a in db.Ajat on op.opettaja_id equals a.opettaja_id
                      where a.aika_id == id
                              select op).FirstOrDefault();

            var varausAika = (from op in db.Ajat
                              join a in db.Opettajat on op.opettaja_id equals a.opettaja_id
                              where op.aika_id== id
                              select op).FirstOrDefault();

            ViewBag.openimi = ope.etunimi +" "+ ope.sukunimi;
            ViewBag.varausalku =varausAika.alku_aika.ToString("dd.MM.yyyy HH:mm");
            ViewBag.varausloppu=varausAika.alku_aika.AddMinutes(varausAika.kesto_id).ToString("HH:mm");


            var varattava = from a in db.Ajat
                            join op in db.Opettajat on a.opettaja_id equals op.opettaja_id
                            join k in db.Kestot on a.kesto_id equals k.kesto_id
                            join v in db.Varaukset on a.aika_id equals v.aika_id
                            into gj
                            from varaus in gj.DefaultIfEmpty()
                            where a.aika_id == id

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
        public ActionResult TeeVaraus([Bind(Include = "aika_id, Varaaja, Aihe")] ajatListaData varaus)
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
                     
                        //EPAONNISTUNUT MODAALI
                        //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                        TempData["Errori"] = "Joku muu varasi juuri tämän ajan.";
                        TempData["BodyText1"] = "Valitsethan toisen ajan.";
                        return RedirectToAction("Index", "Home");
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

                                // Send email opettajalle
                                WebMail.Send(to: ajatOpe.sahkoposti,
                                            subject: "Ohjausaikavaraus Tivi-ohjaussovelluksen kautta",
                                            body: "<b><p>Hei,</p></b>" +
                                            "<p>Sinulle on tehty ohjausajanvaraus Tivi-ohjaus-sovelluksen kautta ajalle " + varausAika.alku_aika.ToString("dd.MM.yyyy") + "klo.  " + varausAika.alku_aika.ToString("HH:mm") + " -" + varausAika.alku_aika.AddMinutes(varausAika.kesto_id).ToString("HH:mm") + ". (kesto " + varausAika.kesto_id + " minuuttia).</p><br><a href="+ varausAika.paikka +">Tapaamisen linkki</a><br><p>Olethan ongelmatilanteissa yhteydessä sovelluksen pääkäyttäjään "+ohjelmanyhteystiedot.PaakayttajaEmailiin+"iin.<br><br>Terveisin, <br> "+ohjelmanyhteystiedot.Terveisin+"<br></p> <a href = "+ohjelmanyhteystiedot.Sivusto+ ">" + ohjelmanyhteystiedot.SivustonNimi + "</a><br><p> Tähän viestiin ei voi vastata.</p> ", isBodyHtml: true
                                        );
                               
                                // Send email varaajalle
                                WebMail.Send(to: varaus.Varaaja,
                                            subject: "Varausvahvistus: Ohjausaika TiVi-opettajalle",
                                            body: "<b><p>Hei,</p></b>" +
                                            "<p>Olet varannut Tivi-ohjaus-sovelluksen kautta ohjausajan opettajalle " + varausAika.Opettajat.etunimi +" "+ varausAika.Opettajat.sukunimi + ". <br>Varauksen aika " + varausAika.alku_aika.ToString("dd.MM.yyyy") + " klo. " + varausAika.alku_aika.ToString("HH:mm") + " - " + varausAika.alku_aika.AddMinutes(varausAika.kesto_id).ToString("HH:mm") + "<br>Tapaamisen kesto on " + varausAika.kesto_id + " minuuttia. <br><a href=" + varausAika.paikka + ">Tapaamisen linkki</a><br>" +
                                            "<p>Tarvittaessa voit perua ajan Tivi-ohjaus-sovelluksen kautta koodilla:  " + varauksesi.id_hash + "<br>Terveisin, <br> "+ohjelmanyhteystiedot.Terveisin+"<br></p>" + "<a href="+ohjelmanyhteystiedot.Sivusto+">"+ohjelmanyhteystiedot.SivustonNimi +"</a><br><p>Tähän viestiin ei voi vastata.</p>", isBodyHtml: true
                                        );
                                ViewBag.Status = "Vahvistusviesti lähetetty antamaasi sähköpostiosoitteeseen. Jos viestiä ei löydy, tarkista roskapostisi.";
                           

                            }
                            catch (Exception)
                            {
                                ViewBag.Status = "Et ole antanut sähköpostiosoitetta.";

                            }
                                Session["varausnro"] = varauksesi.id_hash;
                                Session["varaaja"] = varaus.Varaaja;
                                Session["aihe"] = varaus.Aihe;
                                Session["paikka"] = varauksesi.Ajat.paikka;
                                Session["aika"] = varausAika.alku_aika.ToString("dd.MM.yyyy HH:mm");
                                Session["loppuaika"] = varausAika.alku_aika.AddMinutes(varausAika.kesto_id).ToString("HH:mm");
                                Session["kesto"] = varausAika.kesto_id;
                                //return RedirectToAction("OnnistunutVaraus");

                        }
                        else
                        {
                            ViewBag.Status = "Opettajaa ei löytynyt";
                        }
                    }
               
                    return RedirectToAction("OnnistunutVaraus");


                }

                //Tähän mennee, jos model ei ole validi

                //EionnistunutModaali
                    //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                    TempData["Errori"] = "Varauksen lähetys epäonnistui.";
                    TempData["BodyText1"] = "";
                    
                return RedirectToAction("Index", "Home");

            }
            catch
            {
                //EPAONNISTUNUT MODAALI
                //Annetaan tieto, että jokin meni pieleen TempDatalle modaali-ikkunaa varten
                TempData["Errori"] = "Varauksen tekeminen epäonnistui.";
                TempData["BodyText1"] = "";
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
            ViewBag.SivustonNimi = ohjelmanyhteystiedot.SivustonNimi;
            ViewBag.Alkupvm = varaus.Ajat.alku_aika.ToString("dd.MM.yyyy");
            ViewBag.Alkuaika= varaus.Ajat.alku_aika.ToString("HH:mm");
            ViewBag.Loppuaika = varaus.Ajat.alku_aika.AddMinutes(varaus.Ajat.kesto_id).ToString("HH:mm");

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

    }
}
