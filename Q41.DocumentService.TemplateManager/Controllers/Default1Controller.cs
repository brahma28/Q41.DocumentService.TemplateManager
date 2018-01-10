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
using Q41.DocumentService.TemplateManager.BAL;
//using System.Configuration;
//using System.Web.Configuration;


namespace Q41.DocumentService.TemplateManager.Controllers
{
    public class Default1Controller : Controller
    {
        protected DocumentBuilder DocumentBuilder { get; set; }
        private Q88DocumentServiceTestEntities db = new Q88DocumentServiceTestEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //protected abstract string TemplatePath { get; }
        public byte[] DocumentData { get; private set; }
        //
        // GET: /Default1/

        public ActionResult Index()
        {
            ViewBag.poruka = ""; 
            if (HttpContext.Request.IsAjaxRequest())
            {
                // the controller action was invoked with an AJAX request
                // UČITATI SAMO PREDLOŠKE ODABRANE GRUPE
                ViewBag.predlosci = db.vwTemplates.ToList();
            }
            var templates = db.Templates.Include(t => t.TemplateGroups);
            ViewBag.grupe = db.TemplateGroups.ToList();
            ViewBag.predlosci = db.vwTemplates.ToList();
            return View(templates.ToList());
        }
        [HttpPost]
        public ActionResult Index(Q41.DocumentService.TemplateManager.Models.Templates model)
        {
            ViewBag.poruka = ""; 
            var templates = db.Templates.Include(t => t.TemplateGroups);
            ViewBag.grupe = db.TemplateGroups.ToList();
            ViewBag.predlosci = db.vwTemplates.ToList();
            if (HttpContext.Request.IsAjaxRequest())
            {
                ViewBag.predlosci = db.vwTemplates.Where(t => t.TemplateGroupId == model.TemplateGroupId).ToList();
                templates = db.Templates.Include(t => t.TemplateGroups).Where(t => t.TemplateGroupId == model.TemplateGroupId);
            }
            if (model.TemplateGroupId > 0 )
            {
                ViewBag.predlosci = db.vwTemplates.Where(t => t.TemplateGroupId == model.TemplateGroupId).ToList();
                templates = db.Templates.Include(t => t.TemplateGroups).Where(t => t.TemplateGroupId == model.TemplateGroupId);
            }
            return View(templates.ToList());
        }
        [HttpPost]
        public JsonResult IndexJson(Q41.DocumentService.TemplateManager.Models.Templates model)
        {
            ViewBag.poruka = ""; 

            var templates = db.Templates.Include(t => t.TemplateGroups).Where(t => t.TemplateGroupId == model.TemplateGroupId).ToList();





            return Json(new { templates },
                JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Default1/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.poruka = ""; 
            Templates templates = db.Templates.Find(id);
            if (templates == null)
            {
                return HttpNotFound();
            }
            return View(templates);
        }

        //
        // GET: /Default1/Create

        public ActionResult Create()
        {
            ViewBag.poruka = ""; 
            if (HttpContext.Request.IsAjaxRequest())
            {
                // the controller action was invoked with an AJAX request
                ViewBag.procedure = db.vwProcedureCalls.ToList();
            }
            ViewBag.procedure = db.vwProcedureCalls.ToList();
            ViewBag.templateGroups = db.TemplateGroups.ToList();


            ViewBag.MergeFields = Function.DohvatiPoljaPredloska(0);    // partial view: PartialParametriPredloska.cshtml 
            ViewBag.spFields = Function.DohvatiPoljaProcedure(0);  // partial view: PartialParametriProcedure.cshtml
            ViewBag.BindingFields = Function.DohvatiPoljaMapiranja(0);   // partial view: PartialParametriMapiranja.cshtml

            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name");
            ViewBag.TempID = 1; 
            ViewBag.nazivprocedure = "PROCEDURE";
            ViewBag.nazivpredloska = "PREDLOŠKA";
            return View();
        }
        public ActionResult CreateEdit(int id = 0)
        {
            ViewBag.TemplateId = id;             
            var model = db.Templates.Where(i => i.TemplateId == id).SingleOrDefault();
            if (HttpContext.Request.IsAjaxRequest())
            {
                // AJAX POZIV OVDJE IMAMO SAM NA GUMBU: SPREMI
                string dataDir = ConfigurationManager.AppSettings["putanjaPredloska"];
                string dataDirTemp = ConfigurationManager.AppSettings["putanjaPredloskaPrivremena"];
                if (!System.IO.Directory.Exists(dataDir)) { System.IO.Directory.CreateDirectory(dataDir);}
                if (System.IO.Directory.Exists(dataDirTemp))
                {
                    try
                    {
                        System.IO.File.Copy(System.IO.Path.Combine(dataDirTemp, model.Path), System.IO.Path.Combine(dataDir, model.Path), true);
                        int temId = Function.SpremiPredlozak(id, model.Name, model.Paramaters, dataDirTemp);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Izvorni predložak nije pronađen!  \n{0} ", ex.ToString());
                    }
                }
                if (true)
                {
                    int templateDataSourceId = db.TemplateDataSources.Where(i => i.TemplateId == id).Select(i => i.TemplateDataSourceId).SingleOrDefault();
                    return RedirectToAction("Edit", "Mapiranje", new { Id = templateDataSourceId });
                }
            }

            model.ProcedureCallId = db.TemplateDataSources.Where(i => i.TemplateId == id).Select(i => i.ProcedureCallId).SingleOrDefault();
            ViewBag.predlozak = model.Name; 
            ViewBag.dbo = "dbo";
            ViewBag.stora = db.ProcedureCalls.Where(i => i.ProcedureCallId == model.ProcedureCallId).Select(i => i.ProcedureName).SingleOrDefault();
            ViewBag.poruka = "";
            ViewBag.MergeFields = Function.DohvatiPoljaPredloska(model.TemplateId);    // partial view: PartialParametriPredloska.cshtml 
            
            //ViewBag.spFields = Function.DohvatiPoljaProcedure(model.ProcedureCallId);  // partial view: PartialParametriProcedure.cshtml
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            var jednopolje = db.vwProcedureCalls.Where(i => i.ProcedureCallId == model.ProcedureCallId).Select(i => i.ResultFields).FirstOrDefault();
            try
            {
                rezultaticici = SerializationUtil.Deserialize<Dictionary<string, ProcedureResultField>>(jednopolje);
                rezultaticici = SerializationUtil.expandComplexFields(rezultaticici);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom dohvata parametara store. \n{0}", ex.ToString());
                ViewBag.MessageStora = "Nastala je greška prilikom dohvata parametara store.";
                throw;
            }
            ViewBag.spFields = rezultaticici;   
            //ViewBag.BindingFields = Function.DohvatiPoljaMapiranja(model.TemplateId);   // partial view: PartialParametriMapiranja.cshtml

            TemplateDataSources templatesDS = db.TemplateDataSources.Where(p => p.TemplateId == id).FirstOrDefault();
            if (templatesDS == null)
            {
                templatesDS = Function.DohvatiTemplateDataSources(id);
            }

            SerializationUtil.Deserialize<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);
            List<TemplateFieldBinding> rezultat = new List<TemplateFieldBinding>();
            rezultat = Function.DohvatiPoljaMapiranjaPredloška(id);
            ViewBag.BindingFields = rezultat;


            Dictionary<string, TemplateField> rezultats = new Dictionary<string, TemplateField>();
            rezultats = SerializationUtil.DeserializeFull<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);
            ViewBag.MergeFields = rezultats;  // polja predloska za partial view


            ViewBag.nazivprocedure = "PROCEDURE";
            ViewBag.nazivpredloska = "PREDLOŠKA";
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult CreateEdit(Q41.DocumentService.TemplateManager.Models.Templates model, string Command)
        {
            // deklariranje varijabli:
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            //SqlConnection cnn = new SqlConnection(cnnString);
            //SqlCommand cmd = new SqlCommand();
            string dataDir = ConfigurationManager.AppSettings["putanjaPredloska"];
            string dataDirTemp = ConfigurationManager.AppSettings["putanjaPredloskaPrivremena"];
            ViewBag.poruka = "";

            string fileName1 = "";
            List<ProcedureCall> returnValue = new List<ProcedureCall>();
            List<ProcedureResultField> returnIzlaznaPolja = new List<ProcedureResultField>();
            Document doc = new Document();
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            string pn = ConfigurationManager.AppSettings["defaultStora"];

            string[] spFields = new string[0];
            string[] spMappingFields = new string[0];


            int tgId = 0;

            tgId = model.TemplateGroupId;
            var mergeFields = new List<TemplateParameter>();
            var mergeTemplateFields = new List<TemplateField>();
            Dictionary<string, TemplateField> mergeDicTemplateFields = new Dictionary<string, TemplateField>();


            var spProcedureFields = new List<ProcedureResultField>();
            var spBindingFields = new List<TemplateFieldBinding>();
            ViewBag.nazivprocedure = "PROCEDURE";
            ViewBag.nazivpredloska = "PREDLOŠKA";
            ViewBag.procedure = db.vwProcedureCalls.ToList();
            ViewBag.templateGroups = db.TemplateGroups.ToList();
            ViewBag.MessageStora = "Parametri store uspješno učitani.";
            //
            TemplateDataSources templatesDS = db.TemplateDataSources.Where(p => p.TemplateId == model.TemplateId).FirstOrDefault();

                        //var nesto = SerializationUtil.Deserialize<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);

            ViewBag.BindingFields = SerializationUtil.Deserialize<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);

            if (Command == "SINKRONIZIRAJ")  // trenutno zakomentirano na ovom ekranu
            {
                int broj = 0;
                broj = Function.Sinkroniziraj(model.TemplateId, model.ProcedureCallId);
                if (broj > 0)
                {
                    int templateDataSourceId = db.TemplateDataSources.Where(i => i.TemplateId == model.TemplateId).Select(i => i.TemplateDataSourceId).SingleOrDefault();
                    return RedirectToAction("Edit", "Mapiranje", new { Id = templateDataSourceId });
                }
            }

            if (Command == "SPREMI")
            {


                if (!System.IO.Directory.Exists(dataDir))
                {
                    System.IO.Directory.CreateDirectory(dataDir);
                }
                if (System.IO.Directory.Exists(dataDirTemp))
                {
                    try
                    {
                        System.IO.File.Copy(System.IO.Path.Combine(dataDirTemp, model.Path), System.IO.Path.Combine(dataDir, model.Path), true);
                        int temId = Function.SpremiPredlozak(model.TemplateId, fileName1, model.Paramaters, dataDirTemp);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Izvorni predložak nije pronađen!  \n{0} ", ex.ToString());
                    }
                }

                if (true)
                {
                    int templateDataSourceId = db.TemplateDataSources.Where(i => i.TemplateId == model.TemplateId).Select(i => i.TemplateDataSourceId).SingleOrDefault();
                    return RedirectToAction("Edit", "Mapiranje", new { Id = templateDataSourceId });
                }
            }

            if (HttpContext.Request.IsAjaxRequest())
            {
                // the controller action was invoked with an AJAX request
                ViewBag.procedure = db.vwProcedureCalls.ToList();
            }

            // ****************************************************************************************************************************************************

            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name");
            ViewBag.MergeFields = mergeFields;       // partial view: PartialParametriPredloska.cshtml 
            ViewBag.spFields = spProcedureFields;    // partial view: PartialParametriProcedure.cshtml
            ViewBag.BindingFields = spBindingFields; // partial view: PartialParametriMapiranja.cshtml

            spProcedureFields = Function.ListOfProcedureResultField(spFields); 
            spBindingFields = Function.ListOfTemplateFieldBinding(spMappingFields); 
            
            
            ViewBag.BindingFields = spBindingFields;   // partial view: PartialParametriMapiranja.cshtml
            ViewBag.spFields = rezultaticici;          // partial view: PartialParametriProcedure.cshtml
            ViewBag.nazivprocedure = pn;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]

        //public ActionResult Create(HttpPostedFileBase postedFile)
        public ActionResult Create(Q41.DocumentService.TemplateManager.Models.Templates model, string Command)    
        {
            // deklariranje varijabli:
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            string dataDir = ConfigurationManager.AppSettings["putanjaPredloska"];
            string dataDirTemp = ConfigurationManager.AppSettings["putanjaPredloskaPrivremena"];
            ViewBag.poruka = ""; 
            HttpPostedFileBase postedFile;
            string fileName1 = "";
            List<ProcedureCall> returnValue = new List<ProcedureCall>();
            List<ProcedureResultField> returnIzlaznaPolja = new List<ProcedureResultField>();
            Document doc = new Document();
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();

            string pn = ConfigurationManager.AppSettings["defaultStora"];
            if (model.ProcedureCallId != null)
            { 
                pn = db.ProcedureCalls.Where(i => i.ProcedureCallId == model.ProcedureCallId).Select(i => i.ProcedureName).SingleOrDefault();            
            }


            string[] spFields = new string[0];
            string[] spMappingFields = new string[0];

            string path = "";
            int tgId = 0;

            tgId = model.TemplateGroupId;
            var mergeFields = new List<TemplateParameter>();
            var mergeTemplateFields = new List<TemplateField>();
            Dictionary<string, TemplateField> mergeDicTemplateFields = new Dictionary<string, TemplateField>();

            var tempFields = ""; 
            var spProcedureFields = new List<ProcedureResultField>();
            var spBindingFields = new List<TemplateFieldBinding>();
            ViewBag.nazivprocedure = "PROCEDURE";
            ViewBag.nazivpredloska = "PREDLOŠKA";
            ViewBag.procedure = db.vwProcedureCalls.ToList();
            ViewBag.templateGroups = db.TemplateGroups.ToList();
            ViewBag.MessageStora = "Parametri store uspješno učitani.";
            //

            if (Command == "UČITAJ PREDLOŽAK/PROC.")
            {
                // provjera da li predložak već postoji u bazi, ako da redirect na edit
                postedFile = Request.Files[0];
                fileName1 = Path.GetFileName(postedFile.FileName);
                fileName1 = fileName1.Replace(".docx", "");
                fileName1 = fileName1.Replace(".doc", "");
                model.Name = fileName1;
                ViewBag.Name = fileName1;
                model.Path = fileName1 + ".docx";
                ViewBag.Path = fileName1 + ".docx";
                Templates provjera1 = new Templates(); 
                TemplateDataSources provjera2 = new TemplateDataSources();
                provjera1 = db.Templates.Where(i => i.Name == fileName1).SingleOrDefault();
                if (provjera1 != null) {
                    provjera2 = db.TemplateDataSources.Where(i => i.TemplateId == provjera1.TemplateId).SingleOrDefault();
                    if (provjera2 != null)
                    {
                        ViewBag.poruka = "od prije predložak već ima mapiranje u bazi";
                        return RedirectToAction("Edit", "Mapiranje", new { Id = provjera2.TemplateDataSourceId });
                    }
                    else
                    { 
                        ViewBag.poruka = "predložak se već, od prije, nalazi u bazi"; 
                        return RedirectToAction("Edit", "Default1", new {Id= provjera1.TemplateId});                    
                    }
                }

                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        //uploads[i] = new HttpPostedFileBase();
                        try
                        {
                            postedFile = Request.Files[i];
                            fileName1 = Path.GetFileName(postedFile.FileName);
                            //postedFile.SaveAs(Server.MapPath(fileName1));
                            path = Path.Combine(fileName1, dataDirTemp);
                            // spremanje u temp folder
                            postedFile.SaveAs(dataDirTemp + "\\" + fileName1); // vec ima:  + ".docx"

                            ViewBag.nazivpredloska = fileName1.Substring(0, 24) + "...";
                            ViewBag.Message = "Predložak je uspješno učitan!";
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Nastala je greska prilikom učitavanja predloška : \n{0} ", ex.ToString());
                            ViewBag.Message = ex.Message.ToString();
                        }
                    }
                }

                
                    
                try
                {
                    var izlazi = (from c in db.spProcedureCallSelectByProcedureName(pn) select c.ResultFields).SingleOrDefault();
                    model.ProcedureCallId = (from c in db.spProcedureCallSelectByProcedureName(pn) select c.ProcedureCallId).SingleOrDefault();
                    ViewBag.ProcedureCallId = model.ProcedureCallId;
                    rezultaticici = SerializationUtil.Deserialize<Dictionary<string, ProcedureResultField>>(izlazi);
                    rezultaticici = SerializationUtil.expandComplexFields(rezultaticici);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Nastala je greška prilikom dohvata parametara store. \n{0}", ex.ToString());
                    ViewBag.MessageStora = "Nastala je greška prilikom dohvata parametara store.";
                    throw;
                }


                //if (postedFile != null && postedFile.ContentLength > 0)
                if (fileName1.Length > 0)
                    try
                    {
                        try
                        {
                            doc = new Document(path + "\\" + fileName1);
                            string[] fieldNames = doc.MailMerge.GetFieldNames();
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Nastala je greška prilikom otvaranja predloška (provjeriti putanju u config fileu). \n{0}", ex.ToString());
                            ViewBag.Message = "Nastala je greška prilikom otvaranja predloška (provjeriti putanju u config fileu).";
                            throw;
                        }

                        try
                        {
                            string[] fieldNames = doc.MailMerge.GetFieldNames();
                            // provjera postojanja polja
                            foreach (var mergeField in fieldNames)
                            {
                                mergeFields.Add(new TemplateParameter() { Name = mergeField, DataType = TemplateParameterDataType.nvarchar, IsRequired = true, Description = "", Example = "" });
                            }
                            model.Paramaters = SerializationUtil.Serialize(mergeFields); 
                            ViewBag.MergeFields = mergeFields;    // partial view: PartialParametriPredloska.cshtml  

                            //foreach (var mergeTemplateField in fieldNames)
                            //{
                            //    mergeTemplateFields.Add(new TemplateField() { Name = mergeTemplateField, IsAutoField = false });
                            //}
                            //tempFields = SerializationUtil.Serialize(mergeTemplateFields);


                            foreach (var mergeDicTemplateField in fieldNames)
                            {
                                if (!mergeDicTemplateFields.ContainsKey(mergeDicTemplateField))
                                {
                                    mergeDicTemplateFields.Add(mergeDicTemplateField, new TemplateField() { Name = mergeDicTemplateField, IsAutoField = false });
                                }
                            }
                            tempFields = SerializationUtil.Serialize(mergeDicTemplateFields); 
                            // dodati odmah u bazu

                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Nastala je greška tijekom pretrage mergefield-ova . \n{0}", ex.ToString());
                            throw;
                        }


                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greska prilikom učitavanja predloška : \n{0} ", ex.ToString());
                        ViewBag.Message = ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "Predložak nije pronađena!";
                }

                // izrada zaglavlja (ako ne postoji) registriranje u tabeli: dbo.Templates:
                int temId = Function.RegistrirajPredlozakSaStorom(model.TemplateId, model.ProcedureCallId, fileName1, model.TemplateGroupId, model.Paramaters, tempFields);
                model.TemplateId = db.Templates.Where(i => i.Name == fileName1).Select(i => i.TemplateId).SingleOrDefault();
                ViewBag.TemplateId = model.TemplateId;


            }
            if (Command == "SINKRONIZIRAJ")
            {
                int broj = 0; 
                broj = Function.Sinkroniziraj(model.TemplateId, model.ProcedureCallId); 

                //Response.Redirect("PageA.cshtml");
                // ako je se nešto povezalo odi to vidjeti

                if (broj > 0)
                {
                    int templateDataSourceId = db.TemplateDataSources.Where(i => i.TemplateId == model.TemplateId).Select(i => i.TemplateDataSourceId).SingleOrDefault();

                    // SPREMI start  (NAPOMENA: ako idem na ekran CREATEEDIT onda nece trebati spremati)
                    if (!System.IO.Directory.Exists(dataDir))
                    {
                        System.IO.Directory.CreateDirectory(dataDir);
                    }
                    if (System.IO.Directory.Exists(dataDirTemp))
                    {
                        try
                        {
                            System.IO.File.Copy(System.IO.Path.Combine(dataDirTemp, model.Path), System.IO.Path.Combine(dataDir, model.Path), true);
                            int temId = Function.SpremiPredlozak(model.TemplateId, fileName1, model.Paramaters, dataDirTemp);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Izvorni predložak nije pronađen!  \n{0} ", ex.ToString());
                        }
                    }
                    // SPREMI end
                    return RedirectToAction("Edit", "Mapiranje", new { Id = templateDataSourceId });
                }

            }

            if (Command == "SPREMI")
            {


                if (!System.IO.Directory.Exists(dataDir))
                {
                    System.IO.Directory.CreateDirectory(dataDir);
                }
                if (System.IO.Directory.Exists(dataDirTemp))
                {
                    try {
                        System.IO.File.Copy(System.IO.Path.Combine(dataDirTemp, model.Path), System.IO.Path.Combine(dataDir, model.Path), true);
                        int temId = Function.SpremiPredlozak(model.TemplateId, fileName1, model.Paramaters, dataDirTemp);
                    }
                    catch(Exception ex)
                    {
                         log.ErrorFormat("Izvorni predložak nije pronađen!  \n{0} ", ex.ToString());             
                    }
                }

                if (true)
                {
                    int templateDataSourceId = db.TemplateDataSources.Where(i => i.TemplateId == model.TemplateId).Select(i => i.TemplateDataSourceId).SingleOrDefault();
                    return RedirectToAction("Edit", "Mapiranje", new { Id = templateDataSourceId });
                }
            }



            if (HttpContext.Request.IsAjaxRequest())
            {
                // the controller action was invoked with an AJAX request
                ViewBag.procedure = db.vwProcedureCalls.ToList();
            }

            // ****************************************************************************************************************************************************

            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name");
            ViewBag.MergeFields = mergeFields;       // partial view: PartialParametriPredloska.cshtml 
            ViewBag.spFields = spProcedureFields;    // partial view: PartialParametriProcedure.cshtml
            ViewBag.BindingFields = spBindingFields; // partial view: PartialParametriMapiranja.cshtml

            // izlazna polja procedure: 
            foreach (var spField in spFields)
            {
                spProcedureFields.Add(new ProcedureResultField() { Name = spField });
            }

            // mapirani parovi
            foreach (var spMappingField in spMappingFields)
            {
                spBindingFields.Add(new TemplateFieldBinding() { SourceName = spMappingField });
            }

            ViewBag.BindingFields = spBindingFields;   // partial view: PartialParametriMapiranja.cshtml
            ViewBag.spFields = rezultaticici;    // partial view: PartialParametriProcedure.cshtml
            ViewBag.nazivprocedure = pn;

            return View(model);
        }
        [HttpPost]
        public ActionResult Spremi()
        {
            ViewBag.poruka = ""; 
            int templateId = 111;
            int tempDSId = 0;
            List<TemplateFieldBinding> rezultat = new List<TemplateFieldBinding>();
            rezultat = Function.DohvatiPoljaMapiranjaPredloška(templateId);

            ViewBag.mapiranje = db.vwMapiranja.ToList();
            var spMappingFields = new List<TemplateFieldBinding>();

            foreach (var spMappingField in spMappingFields)
            {
                spMappingFields.Add(new TemplateFieldBinding()
                {
                    SourceName = spMappingField.ToString()
                });
            }

            ViewBag.BindingFields = rezultat; 

            //return Json(new { TemplateDataSourceId = tempDSId },
            //    JsonRequestBehavior.AllowGet);

            return PartialView("PartialParametriMapiranja", ViewBag.BindingFields);

        }
        [HttpPost]
        public ActionResult PartialParametriMapiranja(int id)    
        {
            ViewBag.poruka = "";
            List<TemplateFieldBinding> rezultat = new List<TemplateFieldBinding>();
            rezultat = Function.DohvatiPoljaMapiranjaPredloška(id);
            ViewBag.mapiranje = db.vwMapiranja.ToList();
            var spMappingFields = new List<TemplateFieldBinding>();
            foreach (var spMappingField in spMappingFields)
            {
                spMappingFields.Add(new TemplateFieldBinding() { 
                    SourceName = spMappingField.ToString() 
                });
            }
            ViewBag.test = "test"; 
            return PartialView("PartialParametriMapiranja", ViewBag.BindingFields);
        }

        [HttpPost]
        public ActionResult PartialParametriPredloska()
        {
            ViewBag.poruka = ""; 
            //ViewBag.MergeFields = mergeFields;       // partial view: PartialParametriPredloska.cshtml 


            return View();
        }
        [HttpPost]
        public ActionResult PartialParametriProcedure()
        {
            ViewBag.poruka = ""; 
            string nazivstore = "";
            Dictionary<string, ProcedureResultField> rezultat = new Dictionary<string, ProcedureResultField>();
            try
            {
                var izlaz = (from c in db.spProcedureCallSelectByProcedureName(nazivstore) select c.ResultFields).FirstOrDefault();
                rezultat = SerializationUtil.Deserialize<Dictionary<string, ProcedureResultField>>(izlaz);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom dohvata parametara store. \n{0}", ex.ToString());
                ViewBag.MessageStora = "Nastala je greška prilikom dohvata parametara store.";
                throw;
            }
            ViewBag.spFields = rezultat;    // partial view: PartialParametriProcedure.cshtml
            //ViewBag.BindingFields = rezultat;   
            return View();
        }

        public ViewResult PartialParametriMapiranja2(int id)
        {
            ViewBag.poruka = ""; 
            ProcedureCall procedure = new ProcedureCall();
            string cs = ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            procedure.ConnectionStringValue.Value = cs;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(procedure.ConnectionStringValue.Value))
            {
                using (SqlCommand cmd = new SqlCommand("[spPartialParametriMapiranja]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TemplateDataSourceId", SqlDbType.Int).Value = id;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return View("PartialParametriMapiranja");
        }


        public ActionResult Edit(int id = 0)
        {
            ViewBag.poruka = ""; 
            Templates templates = db.Templates.Find(id);
            // ukoliko nisu parametri predloška ovdje, mogu se pronaci i u drugoj tabeli (zaostavština od Q88)
            TemplateDataSources templatesDS = db.TemplateDataSources.Where(p => p.TemplateId == id).FirstOrDefault();
            if (templatesDS == null)
            { 
                templatesDS = Function.DohvatiTemplateDataSources(id);             
            }


            //templates.Paramaters = templatesDS.TemplateFields; 

            if (templates == null)
            {
                return HttpNotFound();
            }

            Document doc = new Document();
            var mergeFields = new List<TemplateParameter>();

            Dictionary<string, TemplateParameter> rezult = new Dictionary<string, TemplateParameter>();
            Dictionary<string, TemplateParameter> rezult2 = new Dictionary<string, TemplateParameter>();
            Dictionary<string, TemplateField> templateFields = new Dictionary<string, TemplateField>();
            Dictionary<string, TemplateField> rezultaticici = new Dictionary<string, TemplateField>();
            try
            {
                if (templates.Paramaters != null)
                { 
                    rezult = SerializationUtil.DeserializeTemplateParameter<Dictionary<string, TemplateParameter>>(templates.Paramaters);
                }
                if (templatesDS.TemplateFields != null)
                { 
                    templateFields = SerializationUtil.DeserializeTemplateFields<Dictionary<string, TemplateField>>(templatesDS.TemplateFields.ToString());
                    rezultaticici = SerializationUtil.Deserialize<Dictionary<string, TemplateField>>(templatesDS.TemplateFields);               
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom deserijalizacije parametara predloška. \n{0}", ex.ToString());
                ViewBag.MessageStora = "Nastala je greška prilikom deserijalizacije parametara predloška.";
                throw;
            }
            ViewBag.MergeFields = rezultaticici;
            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name", templates.TemplateGroupId);
            return View(templates);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Templates templates)
        {
            ViewBag.poruka = ""; 
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
        // GET: /Default1/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ViewBag.poruka = ""; 
            //Templates templates = db.Templates.Find(id);
            //if (templates == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(templates);
            try
            {
                TemplateDataSources tds = db.TemplateDataSources.Where(t => t.TemplateId == id).SingleOrDefault();
                if (tds != null) 
                { 
                    db.TemplateDataSources.Remove(tds);                
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greska prilikom brisanja predloška : \n{0} \n{1}", ex.ToString(), id);
                return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom brisanja predloška!');</script>");
            }

            try
            {
                Templates templates = db.Templates.Find(id);
                if (templates != null) 
                { 
                    db.Templates.Remove(templates);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greska prilikom brisanja predloška : \n{0} \n{1}", ex.ToString(), id);
                return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom brisanja predloška!');</script>");
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.poruka = ""; 
            try
            {
                TemplateDataSources tds = db.TemplateDataSources.Where(t => t.TemplateId == id).SingleOrDefault();
                db.TemplateDataSources.Remove(tds);
            }
            catch
            {
                throw;
            }

            try
            {
                Templates templates = db.Templates.Find(id);
                db.Templates.Remove(templates);
            }
            catch
            {
                throw;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        //
        // POST: /Default1/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult UcitajPredlozak(string putanja)
        {
            ViewBag.poruka = ""; 
            Template predlozak = new Template();
            predlozak.Path = putanja;
            string dataDir = ConfigurationManager.AppSettings["putanjaPredloska"];
            string dataDirTest = ConfigurationManager.AppSettings["putanjaPredloskaPrivremena"];
            //Microsoft.Office.Interop.Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;
            //var ipersistfile = (System.Runtime.InteropServices.ComTypes.IPersistFile)document;
            string tempfile = Path.GetTempFileName();
            //ipersistfile.Save(tempfile, false);
            //return RedirectToAction("Create");
            Document doc = new Document();
            try
            {
                //doc = new Document(Path.Combine(dataDir, predlozak.Path));
                doc = new Document(predlozak.Path);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom otvaranja predloška (provjeriti putanju u config fileu). \n{0}", ex.ToString());
                //this.Response.Success = false;
                throw;
            }
            return Content("");
            

        }

        private void LoadTemplate()
        {
            //Dohvaćanje i učitavanje predloška
            //Document document = new Document(this.TemplatePath);
            //this.DocumentBuilder = new DocumentBuilder(document);
        }
        private void ConvertToBinary()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                this.DocumentBuilder.Document.Save(stream, SaveFormat.Pdf);
                this.DocumentData = stream.ToArray();
            }
        }

        // DocHelper.cs
        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);

            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object objectType = formatter.Deserialize(stream);
            return objectType;
        }

        //[HttpPost]
        //[ValidateInput(false)]
        public ActionResult Povezi(string parametri)
        {
            //int procId = Function.DohvatiParametarProcedureCallId(parametri); 
            //int tmpId = Function.DohvatiParametarTeplateId(parametri); 
            //string oneProcedureField = Function.DohvatiParametarStore(parametri);
            //string oneTemplateField = Function.DohvatiParametarPredloska(parametri);

            var asa = System.Web.Helpers.Json.Decode(parametri);
            int tmpId = Convert.ToInt32(asa.templateId);
            int procId = Convert.ToInt32(asa.storaId);
            string oneProcedureField = asa.recordToDelete;
            string oneTemplateField = asa.recordToDelete2;

            ViewBag.poruka = Function.DodavanjeVezanogParametra(procId, tmpId, oneProcedureField, oneTemplateField);  
            //ViewBag.TempID = templateID; 
            int templateDataSourceId = db.TemplateDataSources.Where(i => i.TemplateId == tmpId && i.ProcedureCallId == procId).Select(i => i.TemplateDataSourceId).SingleOrDefault();
            // mozda napraviti jedan medjuview: createedit
            //return RedirectToAction("Edit", "Mapiranje", new { Id = templateDataSourceId });     
            return RedirectToAction("CreateEdit", "Default1", new { id = tmpId });    
  
        }
        public ActionResult Sinkroniziraj(int procedureCallId, int templateId)
        {
            ViewBag.poruka = ""; 
            int brojSinkroniziranih = 0; 
            string linknapredanketu = "";
            brojSinkroniziranih = Function.Sinkroniziraj(procedureCallId, templateId); 
            return Json(new { lnk = linknapredanketu },
                            JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        //public ActionResult Create(HttpPostedFileBase postedFile)
        public ActionResult Sinkro(int procedureCallId, int templateId)
        {
            ViewBag.poruka = ""; 
            HttpPostedFileBase postedFile;
            List<ProcedureCall> returnValue = new List<ProcedureCall>();
            List<ProcedureResultField> returnIzlaznaPolja = new List<ProcedureResultField>();
            Document doc = new Document();
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            string pn = ConfigurationManager.AppSettings["defaultStora"];
            string dataDir = ConfigurationManager.AppSettings["putanjaPredloska"];
            string dataDirTest = ConfigurationManager.AppSettings["putanjaPredloskaPrivremena"];
            string[] spFields = new string[0];
            string[] spMappingFields = new string[0];
            string fileName1 = "";
            string path = "";
            int tgId = 0;
            int templateID;
            //tgId = model.TemplateGroupId;
            tgId = 0;
            var mergeFields = new List<TemplateParameter>();
            var spProcedureFields = new List<ProcedureResultField>();
            var spBindingFields = new List<TemplateFieldBinding>();
            ViewBag.nazivprocedure = "PROCEDURE";
            ViewBag.nazivpredloska = "PREDLOŠKA";
            ViewBag.procedure = db.vwProcedureCalls.ToList();
            ViewBag.templateGroups = db.TemplateGroups.ToList();
            ViewBag.MessageStora = "Parametri store uspješno učitani.";
            ViewBag.TemplateGroupId = new SelectList(db.TemplateGroups, "TemplateGroupId", "Name");
            ViewBag.MergeFields = mergeFields;       // partial view: PartialParametriPredloska.cshtml 
            ViewBag.spFields = spProcedureFields;    // partial view: PartialParametriProcedure.cshtml
            ViewBag.BindingFields = spBindingFields; // partial view: PartialParametriMapiranja.cshtml

            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    try
                    {
                        postedFile = Request.Files[i];
                        fileName1 = Path.GetFileName(postedFile.FileName);
                        path = Path.Combine(fileName1, dataDirTest);
                        postedFile.SaveAs(dataDirTest + "\\" + fileName1);
                        ViewBag.nazivpredloska = fileName1.Substring(0, 24) + "...";
                        ViewBag.Message = "Predložak je uspješno učitan!";
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greska prilikom učitavanja predloška : \n{0} ", ex.ToString());
                        ViewBag.Message = ex.Message.ToString();
                    }
                }
            }

            try
            {
                var izlazi = (from c in db.spProcedureCallSelectByProcedureName(pn) select c.ResultFields).FirstOrDefault();
                rezultaticici = SerializationUtil.Deserialize<Dictionary<string, ProcedureResultField>>(izlazi);
                rezultaticici = SerializationUtil.expandComplexFields(rezultaticici);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greška prilikom dohvata parametara store. \n{0}", ex.ToString());
                ViewBag.MessageStora = "Nastala je greška prilikom dohvata parametara store.";
                throw;
            }
            if (fileName1.Length > 0)
                try
                {
                    try
                    {
                        doc = new Document(path + "\\" + fileName1);
                        string[] fieldNames = doc.MailMerge.GetFieldNames();
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška prilikom otvaranja predloška (provjeriti putanju u config fileu). \n{0}", ex.ToString());
                        ViewBag.Message = "Nastala je greška prilikom otvaranja predloška (provjeriti putanju u config fileu).";
                    }


                    try
                    {
                        string[] fieldNames = doc.MailMerge.GetFieldNames();
                        foreach (var mergeField in fieldNames)
                        {
                            mergeFields.Add(new TemplateParameter() { Name = mergeField });
                        }
                        ViewBag.MergeFields = mergeFields;    // partial view: PartialParametriPredloska.cshtml 
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška tijekom pretrage mergefield-ova . \n{0}", ex.ToString());
                        throw;
                    }

                    templateID = Function.AddTemplateHeader(fileName1,dataDirTest,tgId);

                    ViewBag.TempID = templateID;
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Nastala je greska prilikom učitavanja predloška : \n{0} ", ex.ToString());
                    ViewBag.Message = ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "Predložak nije pronađena!";
            }
            // izlazna polja procedure: 
            foreach (var spField in spFields)
            {
                spProcedureFields.Add(new ProcedureResultField() { Name = spField });
            }
            // mapirani parovi
            foreach (var spMappingField in spMappingFields)
            {
                spBindingFields.Add(new TemplateFieldBinding() { SourceName = spMappingField });
            }
            ViewBag.BindingFields = spBindingFields;    // partial view: PartialParametriMapiranja.cshtml
            ViewBag.spFields = rezultaticici;           // partial view: PartialParametriProcedure.cshtml
            ViewBag.nazivprocedure = pn;

            return View();
        }

    }
}