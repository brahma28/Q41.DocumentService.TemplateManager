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
    public class TemplateGroupsController : Controller
    {
        private Q88DocumentServiceTestEntities db = new Q88DocumentServiceTestEntities();

        //
        // GET: /TemplateGroups/

        public ActionResult Index()
        {
            return View(db.TemplateGroups.ToList());
        }

        //
        // GET: /TemplateGroups/Details/5

        public ActionResult Details(int id = 0)
        {
            TemplateGroups templategroups = db.TemplateGroups.Find(id);
            if (templategroups == null)
            {
                return HttpNotFound();
            }
            return View(templategroups);
        }

        //
        // GET: /TemplateGroups/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TemplateGroups/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TemplateGroups templategroups)
        {
            if (ModelState.IsValid)
            {
                db.TemplateGroups.Add(templategroups);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(templategroups);
        }

        //
        // GET: /TemplateGroups/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TemplateGroups templategroups = db.TemplateGroups.Find(id);
            if (templategroups == null)
            {
                return HttpNotFound();
            }
            return View(templategroups);
        }

        //
        // POST: /TemplateGroups/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TemplateGroups templategroups)
        {
            if (ModelState.IsValid)
            {
                db.Entry(templategroups).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(templategroups);
        }

        //
        // GET: /TemplateGroups/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TemplateGroups templategroups = db.TemplateGroups.Find(id);
            if (templategroups == null)
            {
                return HttpNotFound();
            }
            return View(templategroups);
        }

        //
        // POST: /TemplateGroups/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TemplateGroups templategroups = db.TemplateGroups.Find(id);
            db.TemplateGroups.Remove(templategroups);
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