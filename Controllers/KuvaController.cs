using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ajanvarausprojekti.Controllers
{
    public class KuvaController : Controller
    {
        // GET: Kuva
        // GET: Kuva
        public ActionResult Index()
        {
            List<OpekuvaObj> OpekuvaObjFiles = new List<OpekuvaObj>();
            foreach (string strfile in Directory.GetFiles(Server.MapPath("~/Opekuvat")))
            {
                FileInfo fi = new FileInfo(strfile);
                OpekuvaObj obj = new OpekuvaObj();
                obj.Tiedosto = fi.Name;
                obj.Koko = fi.Length;
                obj.Tyyppi = fi.Extension;
                OpekuvaObjFiles.Add(obj);
            }

            return View(OpekuvaObjFiles);
        }

        public ActionResult Delete(string fileName)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Opekuvat"), fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Index(OpekuvaObj doc)
        {

            foreach (var file in doc.files)
            {

                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Opekuvat"), fileName);
                    file.SaveAs(filePath);
                }

            }
            TempData["Message"] = "tiedosto tallennettu onnistuneesti";
            return RedirectToAction("Index");
        }
    }
}

public class OpekuvaObj
{
    public IEnumerable<HttpPostedFileBase> files { get; set; }
    public string Tiedosto { get; set; }
    public long Koko { get; set; }
    public string Tyyppi { get; set; }
}