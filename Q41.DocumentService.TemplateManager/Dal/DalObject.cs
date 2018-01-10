using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;


namespace Q41.DocumentService.TemplateManager.Dal
{
    public class DalObject
    {
        #region Fields

        private const string CONNECTION_STRING_KEY = "DocumentServiceTest";
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Methods

        protected SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings[CONNECTION_STRING_KEY].ConnectionString);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Ne mogu dohvatiti connection string. Message: {0}", e != null ? e.Message : String.Empty);
                throw;
            }

        }

        protected SqlCommand GetCommand(SqlConnection sqlConnection, string commandText, System.Data.CommandType commandType)
        {
            return new SqlCommand()
            {
                Connection = sqlConnection,
                CommandText = commandText,
                CommandType = System.Data.CommandType.StoredProcedure // propustiti mozda ipak parametar
            };
        }

        #endregion

    }
}