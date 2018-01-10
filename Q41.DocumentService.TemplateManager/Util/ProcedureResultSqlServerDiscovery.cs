using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Dal;

using System.Data;

namespace Q41.DocumentService.TemplateManager.Util
{
    //public class ProcedureResultSqlServerDiscovery : IProcedureResultDiscovery
    public class ProcedureResultSqlServerDiscovery 
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // metoda za recover parametara: početak
        public Task<Dictionary<string, ProcedureResultField>> Discover(ProcedureCall procedure, Dictionary<string, object> values)
        {
            Dictionary<string, ProcedureResultField> rez = new Dictionary<string, ProcedureResultField>();
            DataSet ds = new DataSet(); 
            try
            {
                MSSQLDiscovery mssqlDal = new MSSQLDiscovery();

                //ds = await mssqlDal.GenericProcedureCall(procedure, values);
                rez = this.MapResult(ds, procedure);

            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                //todo: ovdje treba javiti neki specifičan exception koji govori više o tome što se dogodilo
                throw;
            }
           
            //return rez;
            return null;

        }

        /// <summary>
        /// Maps the result returned from stored procedure and creates dictionary collection of all fields
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private Dictionary<string, ProcedureResultField> MapResult(DataSet result, ProcedureCall procedure)
        {

            ProcedureResultDiscoveryMapper mapper = new ProcedureResultDiscoveryMapper();
            Dictionary<string, ProcedureResultField> returnValue = mapper.Discover(result, procedure.Paramaters);

            return returnValue;
        }

        public static ProcedureCall DohvatiDefinicijuProcedure(ProcedureCall procedure)
        {
            Dictionary<string, object> values = new Dictionary<string, object>(); 
            DataSet ds = new DataSet(); 
            MSSQLDiscovery mssqlDal = new MSSQLDiscovery();
            //ds = mssqlDal.GenericProcedureCall(procedure, values);
            
            return procedure; 
        }

    }
}