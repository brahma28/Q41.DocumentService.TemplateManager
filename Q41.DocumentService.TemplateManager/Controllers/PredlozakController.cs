using System;
using System.Collections.Generic;
using System.Data;

using System.Data.Entity;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.IO;
using System.Text;
using Aspose.Words;
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Transactions;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;


namespace Q41.DocumentService.TemplateManager.Controllers
{
    public class PredlozakController : Controller
    {
        private Q88DocumentServiceTestEntities db = new Q88DocumentServiceTestEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /Predlozak/

        public ActionResult Index()
        {
            ViewBag.predlosci = db.vwTemplates.ToList();
            var templates = db.Templates.Include(t => t.TemplateGroups);
            return View(templates.ToList());
        }

        //
        // GET: /Predlozak/Details/5

        public ActionResult Details(int id = 0)
        {
            Templates templates = db.Templates.Find(id);
            if (templates == null)
            {
                return HttpNotFound();
            }
            return View(templates);
        }

        //
        // GET: /Predlozak/Create

        public ActionResult Create()
        {
            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name");
            return View();
        }

        //
        // POST: /Predlozak/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Templates templates)
        {
            if (ModelState.IsValid)
            {
                db.Templates.Add(templates);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name", templates.TemplateGroupId);
            return View(templates);
        }

        //
        // GET: /Predlozak/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Templates templates = db.Templates.Find(id);
            if (templates == null)
            {
                return HttpNotFound();
            }
            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name", templates.TemplateGroupId);
            ViewBag.vrstabezepodataka = new VrstaBazePodataka(); 
            return View(templates);
        }

        //
        // POST: /Predlozak/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Templates templates)
        {
            if (ModelState.IsValid)
            {
                db.Entry(templates).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name", templates.TemplateGroupId);
            return View(templates);
        }

        //
        // GET: /Predlozak/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Templates templates = db.Templates.Find(id);
            if (templates == null)
            {
                return HttpNotFound();
            }
            return View(templates);
        }

        //
        // POST: /Predlozak/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Templates templates = db.Templates.Find(id);
            db.Templates.Remove(templates);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}