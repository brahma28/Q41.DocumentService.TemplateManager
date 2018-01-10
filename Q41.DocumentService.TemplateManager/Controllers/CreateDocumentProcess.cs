using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Aspose.Words;
using System.Configuration;
using System.IO;

namespace Q41.DocumentService.TemplateManager.Controllers
{
    public class CreateDocumentProcess : Controller
    {
        //
        // GET: /CreateDocumentProcess/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Loads all template data and all related & child entities. We need everything at this point
        /// </summary>
        //private async Task FetchCompleteTemplate()
        //{
        //    // odi po temnplate (ovo dohvaća odma i datasource
        //    TemplatesDao dao = new TemplatesDao();
        //    this.Template = await dao.SelectSingle(this.Request.Identifier);

        //    if (this.Template == null)
        //    {
        //        log.Error("Template nije pronađen u bazi");
        //        throw new Exception("Template nije pronađen u bazi");
        //    }
        //    // odi po procedurecall (ovo dohvaća odma i conenction string)
        //    ProcedureCallsDao procdao = new ProcedureCallsDao();
        //    this.Procedure = await procdao.SelectSingle(this.Template.DataSource.ProcedureCallId);

        //    if (this.Procedure == null)
        //    {
        //        log.Error("Procedura nije pronađena");
        //        throw new Exception("Procedura nije pronađena");
        //    }
        //}
    }
}
