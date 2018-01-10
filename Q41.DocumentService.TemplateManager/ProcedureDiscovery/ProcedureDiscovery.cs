using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;

namespace Q41.DocumentService.TemplateManager.ProcedureDiscovery
{
    /// <summary>
    /// Klasa koja pokreće proces otkrivanja parametara i izlaznih kursora procedure
    /// </summary>
    /// <remarks>Ovisno o metapodacima odabire se jedna od raspoloživih strategija otkrivanja :
    ///		- DB2 na zOS
    ///		- DB2 van zOS?? (razlikuje se način dohvata parametara procedura)
    ///		- Oracle
    ///		- SqlServer
    ///		
    /// Strategija otkrivanja koristi Factory metodu u ProcedureDiscoveryProvider.Create() statičkoj metodi
    ///		
    ///	Za svaku od tih baza može se dogoditi jedan od scenraija :
    ///		- parametri procedure su poznati (otkriveni nekada prije)
    ///		- parametri procedure nisu poznati (ili se radi refresh)
    ///	
    /// Kada se saznaju parametri može se okinuti procedura da bi se saznao izgled kursora u ovim varijacijama :
    ///		- null parametri (posebno napravljena procedura koja očekuje sve null vrijednsoti da bi vratila prazne kursore)
    ///		- poznate vrijednosti - šalju se konkretne vrijednosti proceduri da bi ona vratila kursore iz kojih se radi popis varijabli
    /// </remarks>
    public class ProcedureDiscovery
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ProcedureCall procedure;

        public ProcedureCall Procedure
        {
            get { return procedure; }

        }

        private ProcedureDiscoveryProvider provider;


        public ProcedureDiscovery(ProcedureCall procedure)
        {
            this.procedure = procedure;
            this.provider = ProcedureDiscoveryProvider.Create(procedure.DbmsType);
            //this.DiscoverParameters = 
            //this.DiscoverResult = 
        }

        /// <summary>
        /// Start the discovery proces
        /// </summary>
        public void DiscoverParameters()
        {
            // discover procedure parameters
            IProcedureParameterDiscovery discovery = this.provider.ParameterDiscovery;
            Dictionary<string, ProcedureParameter> parameters = new Dictionary<string, ProcedureParameter>();
            try
            {
                //var miki = discovery.DiscoverParameters(this.procedure);
                //miki.RunSynchronously();
                //parameters = miki.Result;
                parameters = discovery.DiscoverParameters(this.procedure);
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                //todo: ovdje treba javiti neki specifičan exception koji govori više o tome što se dogodilo
                throw;
            }
            this.procedure.Paramaters = parameters;
        }

        /// <summary>
        /// Attempts to discover the resulting cursors from the procedure. It could be discoverd with null parameters or with exact values that the user supplies
        /// </summary>
        public void DiscoverResult()
        {
            IProcedureResultDiscovery resultDiscovery = this.provider.ResultDiscovery;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, ProcedureResultField> fields = new Dictionary<string, ProcedureResultField>();
            try
            {
                fields = resultDiscovery.Discover(this.procedure, dict);// null strategija za sada   

            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                //todo: ovdje treba javiti neki specifičan exception koji govori više o tome što se dogodilo
                throw;
            }

            this.procedure.ResultFields = fields;

        }
    }
}