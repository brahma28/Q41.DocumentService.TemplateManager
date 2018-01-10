using System;
using System.Collections.Generic;
using System.Data;

using System.Data.Entity;
using System.Data.Entity.Validation;
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
using Q41.DocumentService.TemplateManager.ProcedureDiscovery;


namespace Q41.DocumentService.TemplateManager.Controllers
{
    public class ProceduraController : Controller
    {
        private Q88DocumentServiceTestEntities db = new Q88DocumentServiceTestEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //
        // GET: /Procedura/

        public ActionResult Index()
        {
            //var procedurecalls = db.ProcedureCalls.Include(p => p.ConnectionStrings);
            ViewBag.procedure = db.vwProcedureCalls.ToList();
            var procedurecalls = db.ProcedureCalls;
            return View(procedurecalls.ToList());
        }

        //
        // GET: /Procedura/Details/5

        public ActionResult Details(int id = 0)
        {
            ProcedureCalls procedurecalls = db.ProcedureCalls.Find(id);
            if (procedurecalls == null)
            {
                return HttpNotFound();
            }
            return View(procedurecalls);
        }

        //
        // GET: /Procedura/Create

        public ActionResult Create()
        {
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            ViewBag.spFields = rezultaticici;
            ViewBag.ConnectionStringName = new SelectList(db.ConnectionStrings, "Name", "Value");
            return View();
        }

        //
        // POST: /Procedura/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ProcedureCalls procedurecalls, string Command)
        {
            //string itemKey = "table1.mikimaus";
            //string part1 = "";
            //string part2 = "";
            //part1 = itemKey.Substring(0,itemKey.IndexOf(".")); 
            //part2 = itemKey.Substring(itemKey.IndexOf(".")+1,itemKey.Length-7 ); 

            ViewBag.ConnectionStringName = new SelectList(db.ConnectionStrings.ToList(), "Name", "Value", procedurecalls.ConnectionStringName);   
            if (procedurecalls.ConnectionStringName != null)
            { 
                var ddlCS1 = db.ConnectionStrings.Where(i => i.Name == procedurecalls.ConnectionStringName).ToList();
                var ddlCS2 = db.ConnectionStrings.Where(i => i.Name != procedurecalls.ConnectionStringName).ToList();
                var ddlCS = ddlCS1.Concat(ddlCS2);    
                ViewBag.ConnectionStringName = new SelectList(ddlCS, "Name", "Value", procedurecalls.ConnectionStringName);        
            }
            ViewBag.poruka = "";
            string cnnString = String.Empty; 
            bool proba =  false; 
            //procedurecalls = db.ProcedureCalls.Find(procedurecalls.ProcedureCallId);
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            ViewBag.spFields = rezultaticici;
            Dictionary<string, ProcedureInputParameter> ulazniparametristore = new Dictionary<string, ProcedureInputParameter>();
            cnnString = db.ConnectionStrings.Where(m => m.Name == procedurecalls.ConnectionStringName).Select(u => u.Value).SingleOrDefault().ToString();
            if (cnnString != null || cnnString != String.Empty)
            {
                proba = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.TestConnection(cnnString);
                if (!proba)
                {
                    ViewBag.poruka = "Neuspjelo spajanje na bazu! ";
                    return View(procedurecalls);

                }
                else
                { 
                    // provjera da li je stora već u bazi
                    ProcedureCalls stora = db.ProcedureCalls.Where(m => m.ProcedureName == procedurecalls.ProcedureName).SingleOrDefault();
                    if (stora != null)
                    { 
                        ViewBag.poruka = "Ta pisana procedura već postoji u bazi! ( Obrišite je ili odite na ekran za uređivanje)";
                        return View(procedurecalls);
                    }

                }

            }
            ViewBag.ConnectionStringName = new SelectList(db.ConnectionStrings, "Name", "Value", procedurecalls.ConnectionStringName);


            if (Command == "SPREMI")
            {
                procedurecalls.PackageName = String.Empty; 
                procedurecalls.DbmsType = 3;
                string poruka = Function.ProcedureCallUpsert(0,procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName, procedurecalls.ConnectionStringName, procedurecalls.DbmsType, procedurecalls.Paramaters, procedurecalls.ResultFields);

                ViewBag.spFields = rezultaticici;
                //return View(procedurecalls);
                return RedirectToAction("Index", "Procedura");
            }
            if (Command == "DOHVATI DEFINICIJU PROCEDURE")
            {

                ViewBag.poruka = "";
                if (procedurecalls.ProcedureName == null)
                {
                        ViewBag.poruka = "Morate unijeti naziv procedure.";
                        return View(procedurecalls);
                }
                else
                { 
                    if (procedurecalls.ProcedureName.Trim() == "")
                    {
                        ViewBag.poruka = "Morate unijeti naziv procedure.";
                        return View(procedurecalls);
                    }                
                }
                proba = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.TestConnection(cnnString);
                if (proba) 
                { 
                    // dohvaćanje ulaznih parametara store
                    ulazniparametristore = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.DohvatiUlazneParametreStoreDictionary(cnnString, procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName);
                    procedurecalls.Paramaters = SerializationUtil.Serialize(ulazniparametristore);
                    int brojUlaznihParametara = ulazniparametristore.Count(); 
                    // izlazni parametri store
                    rezultaticici = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.DohvatiResultFieldParametreStoreDictionary(cnnString, procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName);
                    if (rezultaticici.Count() > 0)
                    {
                        rezultaticici = SerializationUtil.expandComplexFields(rezultaticici);
                        procedurecalls.ResultFields = SerializationUtil.Serialize(rezultaticici);
                    }
                    else
                    {
                        ViewBag.poruka = "Ne pronalazim proceduru '" + procedurecalls.SchemaName + "." + procedurecalls.ProcedureName + "' (ili procedura nema izlaznih parametara). Provjerite u bazi!";
                        return View(procedurecalls);                
                    }


                }

                



                procedurecalls.PackageName = String.Empty; // MSSQL server ovo nema
                if (ModelState.IsValid)
                {
                    db.Entry(procedurecalls).State = EntityState.Modified;
                    //try
                    //{
                    //    db.SaveChanges();
                    //}
                    //catch (DbEntityValidationException e)
                    //{
                    //    foreach (var eve in e.EntityValidationErrors)
                    //    {
                    //        log.Fatal("Provjera ispravnosti podataka modela je pronašla grešku! Entitet tipa: " + eve.Entry.Entity.GetType().Name + ", in state " + eve.Entry.State + ", ima grešku:");
                    //        foreach (var ve in eve.ValidationErrors)
                    //        {
                    //            log.Fatal("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage);
                    //        }
                    //    }
                    //    throw;
                    //}
                }
                ViewBag.spFields = rezultaticici;
                ViewBag.ResultFields = procedurecalls.ResultFields;
                ViewBag.Paramaters = procedurecalls.Paramaters;
                //procedurecalls.Paramaters = String.Empty; // ovo je bitno razumijeti !!! 
                return View(procedurecalls);
            }            
            return View(procedurecalls);
        }

        //
        // GET: /Procedura/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ProcedureCalls procedurecalls = db.ProcedureCalls.Find(id);
            procedurecalls = db.ProcedureCalls.Find(id); 
            ViewBag.poruka = "";
            ViewBag.testSpajanjaNaBazu = ""; 
            bool proba = false;
            string cnnString = String.Empty;
            if (db.ConnectionStrings.Where(m => m.Name == procedurecalls.ConnectionStringName).Count() > 0)
            {
                cnnString = db.ConnectionStrings.Where(m => m.Name == procedurecalls.ConnectionStringName).Select(u => u.Value).SingleOrDefault().ToString();
                if (cnnString != null || cnnString != String.Empty)
                {
                    proba = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.TestConnection(cnnString);
                    if(proba){ ViewBag.testSpajanjaNaBazu = "ok"; }
                    else { ViewBag.testSpajanjaNaBazu = "neuspjelo spajanje !"; }
                }
            }
            else
            {
                ViewBag.testSpajanjaNaBazu = "nepostojeći podaci za spajanje na bazu ! (provjeriti tabelu: [dbo].[ConnectionStrings])"; 
            }



            List<ProcedureResultField> procRF = new List<ProcedureResultField>();
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            Dictionary<string, ProcedureComplexResultField> rezultaKomplex = new Dictionary<string, ProcedureComplexResultField>();

            if (procedurecalls == null)
            {
                return HttpNotFound();
            }
            RegistarBazaPodataka rbp = new RegistarBazaPodataka();
            IEnumerable<VrstaBazePodataka> actionTypes = Enum.GetValues(typeof(VrstaBazePodataka))
                                             .Cast<VrstaBazePodataka>();
            rbp.ActionsList = from action in actionTypes
                             select new SelectListItem
                             {
                                 Text = action.ToString(),
                                 Value = action.ToString()
                             };
            ViewBag.rbp = rbp;
            ViewBag.ConnectionStrings = db.ConnectionStrings.ToList();
            var ddlCS1 = db.ConnectionStrings.Where(i => i.Name == procedurecalls.ConnectionStringName).ToList();
            var ddlCS2 = db.ConnectionStrings.Where(i => i.Name != procedurecalls.ConnectionStringName).ToList();
            var ddlCS = ddlCS1.Concat(ddlCS2);
            ViewBag.ConnectionStringName = new SelectList(ddlCS, "Name", "Value", procedurecalls.ConnectionStringName);

            ViewBag.detaljiprocedure = db.vwProcedureCalls.Where(i => i.ProcedureCallId == id).FirstOrDefault();
            ViewBag.vrstabezepodataka = new VrstaBazePodataka();
            var jednopolje = db.vwProcedureCalls.Where(i => i.ProcedureCallId == id).Select(i => i.ResultFields).FirstOrDefault();

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
            ViewBag.spFields = rezultaticici;    // 
            procedurecalls.Paramaters = String.Empty; // ovo je bitno razumijeti !!! 

            string itemKey = "Tabela1.odnosi";
            string puki = (itemKey.IndexOf(".") > 0) ? itemKey.Substring(0, 6) : String.Empty;

            return View(procedurecalls);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProcedureCalls procedurecalls, string Command)
        {
            ViewBag.testSpajanjaNaBazu = ""; 
            ViewBag.poruka = "";
            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            Dictionary<string, ProcedureInputParameter> ulazniparametristore = new Dictionary<string, ProcedureInputParameter>();
            if (Command == "SPREMI")
                {
                    //Response.Redirect("PageA.cshtml");
                }
            if (Command == "DODAJ NOVO POLJE")
                {

                    int procedureCallId = procedurecalls.ProcedureCallId;
                    string parametar = procedurecalls.Paramaters;

                    //ViewBag.poruka = "Parametar: " + parametar + ", uspješno dodan";
                    string poruka = Function.DodajNovoPolje(procedureCallId,parametar,procedurecalls.NewCursorName);
                    ViewBag.poruka = poruka;

                }
            if (Command == "DOHVATI DEFINICIJU PROCEDURE" || Command == "DOHVATI PONOVO POLJA PROCEDURE")
                {

                    string cnnString = db.ConnectionStrings.Where(m => m.Name == procedurecalls.ConnectionStringName).Select(u => u.Value).SingleOrDefault().ToString();



                    //List<ProcedureParameter> parametriIzlazni = new List<ProcedureParameter>();
                    //parametriIzlazni = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.DohvatiParametreStore(cnnString, procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName);
                    
                    //List<ProcedureResultField> parametriResultField = new List<ProcedureResultField>();
                    //parametriResultField = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.DohvatiResultFieldParametreStore(cnnString, procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName);

                    rezultaticici = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.DohvatiResultFieldParametreStoreDictionary(cnnString, procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName);
                    rezultaticici = SerializationUtil.expandComplexFields(rezultaticici);

                    procedurecalls.ResultFields = SerializationUtil.Serialize(rezultaticici);
                    ulazniparametristore = Q41.DocumentService.TemplateManager.Dal.MSSQLDiscovery.DohvatiUlazneParametreStoreDictionary(cnnString, procedurecalls.SchemaName, procedurecalls.PackageName, procedurecalls.ProcedureName);
                    procedurecalls.Paramaters = SerializationUtil.Serialize(ulazniparametristore); 
                    procedurecalls.PackageName = String.Empty; // MSSQL server ovo nema
                    if (ModelState.IsValid)
                    {
                        db.Entry(procedurecalls).State = EntityState.Modified;
                        try 
                        { 
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                log.Fatal("Provjera ispravnosti podataka modela je pronašla grešku! Entitet tipa: " + eve.Entry.Entity.GetType().Name + ", in state " + eve.Entry.State + ", ima grešku:");
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    log.Fatal("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage ); 
                                }
                            }
                            throw;
                        }
                    }

                    ViewBag.spFields = rezultaticici;

                    procedurecalls.Paramaters = String.Empty; // ovo je bitno razumijeti !!! 
                    return View(procedurecalls);


                }

            if (ModelState.IsValid)
            {
                //db.Entry(procedurecalls).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            if (procedurecalls.ProcedureCallId == 0) 
            {
                return new EmptyResult();
            }
            var jednopolje = db.vwProcedureCalls.Where(i => i.ProcedureCallId == procedurecalls.ProcedureCallId).Select(i => i.ResultFields).FirstOrDefault();
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
            procedurecalls.Paramaters = String.Empty; // ovo je bitno razumijeti !!! 
            ViewBag.vrstabezepodataka = new VrstaBazePodataka();
            ViewBag.ConnectionStringName = new SelectList(db.ConnectionStrings, "Name", "Value", procedurecalls.ConnectionStringName);
            return View(procedurecalls);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(ProcedureCalls procedurecalls)
        {
            // potrebno je spremini u pravi folder predlozak


            if (ModelState.IsValid)
            {
                db.Entry(procedurecalls).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.VrstaBazePodaaka = 
            ViewBag.ConnectionStringName = new SelectList(db.ConnectionStrings, "Name", "Value", procedurecalls.ConnectionStringName);
            return View(procedurecalls);
        }
        //
        // GET: /Procedura/Delete/5

        public ActionResult Delete(int id = 0)
        {
            try
            {
                TemplateDataSources tds = db.TemplateDataSources.Where(t => t.ProcedureCallId == id).SingleOrDefault();
                if (tds != null)
                {
                    db.TemplateDataSources.Remove(tds);                
                }

            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greska prilikom brisanja procedure : \n{0} \n{1}", ex.ToString(), id);
                return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom brisanja procedure!');</script>");
            }

            try
            {
                ProcedureCalls procedureCalls = db.ProcedureCalls.Find(id);
                db.ProcedureCalls.Remove(procedureCalls);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greska prilikom brisanja procedure : \n{0} \n{1}", ex.ToString(), id);
                return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom brisanja procedure!');</script>");
            }
            db.SaveChanges();
            return RedirectToAction("Index");


        }


        public ActionResult DeleteIzlazniParametarProcedure(string parametri)
        {

            Dictionary<string, ProcedureResultField> rezultaticici = new Dictionary<string, ProcedureResultField>();
            ViewBag.poruka = "";
            int procedureCallId = 0;
            string parametar = "";

            try
            {
            procedureCallId = Int32.Parse(parametri.Substring(0, parametri.IndexOf("___") ));
            parametar = parametri.Substring(parametri.IndexOf("___") + 3, parametri.Length - parametri.IndexOf("___") - 3);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greska prilikom parsiranja: \n{0} \n{1}", ex.ToString(), parametri);
                return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom parsiranja!');</script>");
            }

            ProcedureCalls procedureCalls = new ProcedureCalls();
            try
            {
                procedureCalls = db.ProcedureCalls.Find(procedureCallId);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Nastala je greska prilikom brisanja izlaznog parametra procedure : \n{0} \n{1}", ex.ToString(), parametri);
                return Content("<script language='javascript' type='text/javascript'>alert('Nastala je greska prilikom brisanja izlaznog parametra procedure!');</script>");
            }
            var ddlCS1 = db.ConnectionStrings.Where(i => i.Name == procedureCalls.ConnectionStringName).ToList();
            var ddlCS2 = db.ConnectionStrings.Where(i => i.Name != procedureCalls.ConnectionStringName).ToList();
            var ddlCS = ddlCS1.Concat(ddlCS2);
            ViewBag.ConnectionStringName = new SelectList(ddlCS, "Name", "Value", procedureCalls.ConnectionStringName);

            var jednopolje = db.vwProcedureCalls.Where(i => i.ProcedureCallId == procedureCalls.ProcedureCallId).Select(i => i.ResultFields).FirstOrDefault();
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
            ViewBag.poruka = "Parametar: " + parametar + ", uspješno obrisan!";

            string poruka = Function.ObrisiParametarProcedure( procedureCallId, parametar);


            jednopolje = db.vwProcedureCalls.Where(i => i.ProcedureCallId == procedureCalls.ProcedureCallId).Select(i => i.ResultFields).FirstOrDefault();
            rezultaticici = SerializationUtil.Deserialize<Dictionary<string, ProcedureResultField>>(jednopolje);
            rezultaticici = SerializationUtil.expandComplexFields(rezultaticici);
            ViewBag.spFields = rezultaticici;   

            db.SaveChanges();
            //return RedirectToAction("Index");
            return View("Edit", procedureCalls);
        }
        //
        // POST: /Procedura/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProcedureCalls procedurecalls = db.ProcedureCalls.Find(id);
            db.ProcedureCalls.Remove(procedurecalls);
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