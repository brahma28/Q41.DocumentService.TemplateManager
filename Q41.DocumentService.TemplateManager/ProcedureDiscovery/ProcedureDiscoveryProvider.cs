using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;

namespace Q41.DocumentService.TemplateManager.ProcedureDiscovery
{
    public class ProcedureDiscoveryProvider
    {
        private IProcedureParameterDiscovery parameterDiscovery = null;
        public IProcedureParameterDiscovery ParameterDiscovery { get { return this.parameterDiscovery; } }

        private IProcedureResultDiscovery resultDiscovery = null;
        public IProcedureResultDiscovery ResultDiscovery { get { return this.resultDiscovery; } }



        public static ProcedureDiscoveryProvider Create(DbmsType dbType)
        {
            ProcedureDiscoveryProvider rez = new ProcedureDiscoveryProvider();
            switch (dbType)
            {
                case DbmsType.Oracle:
                    //rez.parameterDiscovery = new ProcedureParameterOracleDiscovery();
                    //rez.resultDiscovery = new ProcedureResultOracleDiscovery();
                    break;
                case DbmsType.SqlServer:
                    //rez.parameterDiscovery = new ProcedureParameterSqlServerDiscovery();
                    //rez.resultDiscovery = new ProcedureResultSqlServerDiscovery();
                    break;

                case DbmsType.Db2zOS:
                    //rez.parameterDiscovery = new ProcedureParameterDB2Discovery();
                    //rez.resultDiscovery = new ProcedureResultDB2Discovery();
                    break;


                default:
                    //TODO. Odradi za ostale vrste baza
                    //LogManager.GetCurrentClassLogger().Fatal(ex);
                    throw new NotImplementedException("DBMS type not supported");
            }

            return rez;


        }

    }
}