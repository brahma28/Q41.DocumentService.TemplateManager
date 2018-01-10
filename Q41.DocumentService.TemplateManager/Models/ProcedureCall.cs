using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Q41.DocumentService.TemplateManager.Models
{
    public class ProcedureCall
    {
        #region Properties

        public ConnectionString ConnectionStringValue { get; set; }
        //public string ConnectionStringValue { get; set; }

        public DbmsType DbmsType { get; set; }
        public string PackageName { get; set; }
        public Dictionary<string, ProcedureParameter> Paramaters { get; set; }
        public int? ProcedureCallId { get; set; }
        public string ProcedureName { get; set; }
        public Dictionary<string, ProcedureResultField> ResultFields { get; set; }
        public string SchemaName { get; set; }
        //public bool Complex { get; set; }
        //public string ProcedureParameters { get; set; }

        #endregion

        #region Constructors

        public ProcedureCall()
        {
            this.ProcedureCallId = -1;
            this.Paramaters = new Dictionary<string, ProcedureParameter>();
            this.ResultFields = new Dictionary<string, ProcedureResultField>();
        }

        public ProcedureCall(ConnectionString connectionStringValue, DbmsType dbmsType, string packageName, int procedureCallId, string procedureName, string schemaName)
            : this()
        {
            InitializeObject(connectionStringValue, dbmsType, packageName, procedureCallId, procedureName, schemaName);
        }

        public ProcedureCall(ConnectionString connectionStringValue,
            DbmsType dbmsType,
            string packageName,
            Dictionary<string, ProcedureParameter> parameters,
            int procedureCallId,
            string procedureName,
            Dictionary<string, ProcedureResultField> resultFields,
            string schemaName)
        {
            InitializeObject(connectionStringValue, dbmsType, packageName, procedureCallId, procedureName, schemaName);
            this.Paramaters = parameters;
            this.ResultFields = resultFields;
        }

        private void InitializeObject(ConnectionString connectionStringValue, DbmsType dbmsType, string packageName, int procedureCallId, string procedureName, string schemaName)
        {
            this.ConnectionStringValue = connectionStringValue;
            this.DbmsType = dbmsType;
            this.PackageName = packageName;
            this.ProcedureCallId = procedureCallId;
            this.ProcedureName = procedureName;
            this.SchemaName = schemaName;
        }

        #endregion
    }
}
