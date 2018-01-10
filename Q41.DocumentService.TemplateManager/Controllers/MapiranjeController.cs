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

//using Json; 

using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;
using Q41.DocumentService.TemplateManager.BAL;

namespace Q41.DocumentService.TemplateManager.Controllers
{
    public class MapiranjeController : Controller
    {
        private Q88DocumentServiceTestEntities db = new Q88DocumentServiceTestEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //
        // GET: /Mapiranje/

        public ActionResult Index()
        {
            ViewBag.mapiranja = db.vwMapiranja.ToList();
            var templatedatasources = db.TemplateDataSources;
            return View(templatedatasources.ToList());
        }

        //
        // GET: /Mapiranje/Details/5

        public ActionResult Details(int id = 0)
        {
            TemplateDataSources templatedatasources = db.TemplateDataSources.Find(id);
            if (templatedatasources == null)
            {
                return HttpNotFound();
            }
            return View(templatedatasources);
        }

        //
        // GET: /Mapiranje/Create

        public ActionResult Create()
        {
            ViewBag.poruka = ""; 
            ViewBag.ProcedureCallId = new SelectList(db.ProcedureCalls, "ProcedureCallId", "SchemaName");
            ViewBag.TemplateId = new SelectList(db.Templates, "TemplateId", "Name");
            return View();
        }

        //
        // POST: /Mapiranje/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TemplateDataSources templatedatasources)
        {
            ViewBag.poruka = ""; 
            if (ModelState.IsValid)
            {
                db.TemplateDataSources.Add(templatedatasources);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProcedureCallId = new SelectList(db.ProcedureCalls, "ProcedureCallId", "SchemaName", templatedatasources.ProcedureCallId);
            ViewBag.TemplateId = new SelectList(db.Templates, "TemplateId", "Name", templatedatasources.TemplateId);
            return View(templatedatasources);
        }

        //
        // GET: /Mapiranje/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.poruka = ""; 
            TemplateDataSources templatesDS = db.TemplateDataSources.Where(p => p.TemplateDataSourceId == id).FirstOrDefault();
            SerializationUtil.Deserialize<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);
            ViewBag.MergeFields = SerializationUtil.Deserialize<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);
            ViewBag.prcId = db.TemplateDataSources.Where(i => i.TemplateDataSourceId == id).Select(i => i.ProcedureCallId).FirstOrDefault();
            ViewBag.tmpId = db.TemplateDataSources.Where(i => i.TemplateDataSourceId == id).Select(i => i.TemplateId).FirstOrDefault();
            ViewBag.poruka = "";




            TemplateDataSources templatedatasources = db.TemplateDataSources.Find(id);
            if (templatedatasources == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProcedureCallId = new SelectList(db.ProcedureCalls, "ProcedureCallId", "SchemaName", templatedatasources.ProcedureCallId);
            ViewBag.stora = db.ProcedureCalls.Where(t => t.ProcedureCallId == templatedatasources.ProcedureCallId).Select(t => t.ProcedureName).SingleOrDefault();     
            ViewBag.dbo = db.ProcedureCalls.Where(t => t.ProcedureCallId == templatedatasources.ProcedureCallId).Select(t => t.SchemaName).SingleOrDefault();                              
            ViewBag.TemplateId = new SelectList(db.Templates, "TemplateId", "Name", templatedatasources.TemplateId);
            ViewBag.predlozak = db.Templates.Where(t => t.TemplateId == templatedatasources.TemplateId).Select(t => t.Name).SingleOrDefault();

            int templateId = db.TemplateDataSources.Where(t => t.TemplateDataSourceId == templatedatasources.TemplateDataSourceId).Select(t => t.TemplateId).SingleOrDefault();
            int tempDSId = id; 
            List<TemplateFieldBinding> rezultat = new List<TemplateFieldBinding>();
            rezultat = Function.DohvatiPoljaMapiranjaPredloška(templateId);
            ViewBag.BindingFields = rezultat;

            //ViewBag.popisParam = db.spPopisParametara(templatedatasources.ProcedureCallId);
            // POLJA PROCEDURE za potrebe DDL
            Dictionary<string, ProcedureResultField> psp = new Dictionary<string, ProcedureResultField>();
            var jednopolje = db.vwProcedureCalls.Where(i => i.ProcedureCallId == templatedatasources.ProcedureCallId).Select(i => i.ResultFields).FirstOrDefault();
            try
            {
                psp = SerializationUtil.Deserialize<Dictionary<string, ProcedureResultField>>(jednopolje);
                //rezultaKomplex = SerializationUtil.DeserializeComplex<Dictionary<string, ProcedureComplexResultField>>(jednopolje);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom dohvata izlaznih parametara store. \n{0}", ex.ToString());
                ViewBag.MessageStora = "Nastala je greška prilikom dohvata izlaznih parametara store.";
                throw;
            }
            ViewBag.popisParam = psp;   
            //ViewBag.popisPolja = db.spPopisPolja(templateId);

            // POLJA PREDLOŠKA za potrebe DDL
            Dictionary<string, TemplateParameter> ppr = new Dictionary<string, TemplateParameter>();
            var jednopoljepredloska = db.Templates.Where(i => i.TemplateId == templateId).Select(i => i.Paramaters).FirstOrDefault();
            try
            {
                ppr = SerializationUtil.DeserializeTemplateParameter<Dictionary<string, TemplateParameter>>(jednopoljepredloska);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom dohvata izlaznih parametara predloška. [dbo].[Template] \n{0}", ex.ToString());
                ViewBag.MessageStora = "Nastala je greška prilikom dohvata izlaznih parametara predloška. [dbo].[Template]";
                throw;
            }
            if (ppr.Count() == 0)
            {
                // dohvatiti parametre: FROM [Q88DocumentServiceTest].[dbo].[TemplateDataSources].[TemplateFields]
                Dictionary<string, TemplateField> templateFields = new Dictionary<string, TemplateField>();
                var poljetds = db.TemplateDataSources.Where(i => i.TemplateId == templateId).Select(i => i.TemplateFields).FirstOrDefault();       
                try
                {
                    templateFields = SerializationUtil.DeserializeTemplateFields<Dictionary<string, TemplateField>>(poljetds.ToString());
                    //templateFields = SerializationUtil.DeserializeTemplateParameter2<Dictionary<string, TemplateField>>(poljetds.ToString());
                    var mergeFields = SerializationUtil.DeserializeTemplateParameter3<List<TemplateParameter>>(poljetds.ToString());
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Nastala je greška prilikom dohvata izlaznih parametara predloška. [dbo].[TemplateDataSources] \n{0}", ex.ToString());
                    ViewBag.MessageStora = "Nastala je greška prilikom dohvata izlaznih parametara predloška. [dbo].[TemplateDataSources]";
                    throw;
                }
            }
            ViewBag.popisPolja= ppr;   
            // 
            return View(templatedatasources);
        }

        //
        // POST: /Mapiranje/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TemplateDataSources templatedatasources, string Command)
        {
            ViewBag.poruka = "";  
            ViewBag.prcId = db.TemplateDataSources.Where(i => i.TemplateDataSourceId == templatedatasources.TemplateDataSourceId).Select(i => i.ProcedureCallId).FirstOrDefault();
            ViewBag.tmpId = db.TemplateDataSources.Where(i => i.TemplateDataSourceId == templatedatasources.TemplateDataSourceId).Select(i => i.TemplateId).FirstOrDefault();
            if (Command == "SPREMI")
            {
                // poveži storom 
                ViewBag.poruka = Function.DodavanjeVezanogParametra(templatedatasources.ProcedureCallId, templatedatasources.TemplateId, templatedatasources.OneProcedureField, templatedatasources.OneTemplateField);  
            }

            if (ModelState.IsValid)
            {
                db.Entry(templatedatasources).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProcedureCallId = new SelectList(db.ProcedureCalls, "ProcedureCallId", "SchemaName", templatedatasources.ProcedureCallId);
            ViewBag.TemplateId = new SelectList(db.Templates, "TemplateId", "Name", templatedatasources.TemplateId);
            return View(templatedatasources);
        }

        //
        // GET: /Mapiranje/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TemplateDataSources templatedatasources = db.TemplateDataSources.Find(id);
            if (templatedatasources == null)
            {
                return HttpNotFound();
            }
            return View(templatedatasources);
        }

        //
        // POST: /Mapiranje/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // iz tabele: dbo.Templates ga ne brišem ! ( a možda bih trebao ) 
            TemplateDataSources templatedatasources = db.TemplateDataSources.Find(id);
            db.TemplateDataSources.Remove(templatedatasources);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult DeleteParamaterFieldMapping(string parametri)
        {
            ViewBag.poruka = ""; 
            TemplateDataSources model = new TemplateDataSources(); 
            var asa = System.Web.Helpers.Json.Decode(parametri);

            int tmpId = Convert.ToInt32(asa.templateId);
            int procId = Convert.ToInt32(asa.storaId);
            string oneProcedureField = asa.recordToDelete;
            string oneTemplateField = asa.recordToDelete2;

            int broj = Function.ObrisiVezuMapiranjaIzmedjuPolja(tmpId, procId, oneProcedureField, oneTemplateField);
            if (broj > 0)
            {
                ViewBag.poruka = "Uspješno raspareno (" + oneProcedureField + " - " + oneTemplateField + ")"; 
            }
            model = db.TemplateDataSources.Where(i => i.TemplateId == tmpId).SingleOrDefault();

            //try
            //{
            //    // dio koji poziva storu za brisanje mapirane veze

            //}
            //catch (Exception ex)
            //{
            //    log.ErrorFormat("Nastala je greska prilikom kidanja mapirane veze : \n{0} \n{1}", ex.ToString(), parametri);
            //    return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom kidanja mapirane veze!');</script>");
            //}

            return RedirectToAction("Edit", model);
        }
    }
}