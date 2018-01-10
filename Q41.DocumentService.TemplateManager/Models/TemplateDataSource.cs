using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Q41.DocumentService.TemplateManager.Models
{
    [DataContract]
    [KnownType(typeof(ComplexTemplateField))]
    public class TemplateDataSource
    {
        #region Properties

        [DataMember]
        public int TemplateId { get; set; }

        [DataMember]
        public int ProcedureCallId { get; set; }

        [DataMember]
        public int TemplateDataSourceId { get; set; }

        [DataMember]
        public Dictionary<string, TemplateField> TemplateFields { get; set; }

        /// <summary>
        /// Sadrži bidinge između parametara predloška (TemplateParameter) i ProcedureParameter
        /// </summary>
        [DataMember]
        public List<TemplateParameterProcedureCallBinding> ProcedureCallBidings { get; set; }
        //public Dictionary<string, TemplateParameterProcedureCallBinding> ProcedureCallBidings { get; set; }

        /// <summary>
        /// Sadrži bidinge između polja na samom predlošku (TemplateField) sa ulaznim parametrima predloška (TemplateParameter) i/ili izlaznim podacima iz procedure (ProcedureResultField)
        /// </summary>
        [DataMember]
        public List<TemplateFieldBinding> TemplateFieldBidings { get; set; }
        //public Dictionary<string, TemplateFieldBinding> TemplateFieldBidings { get; set; }
        #endregion

        #region Constructors

        public TemplateDataSource()
        {
            this.TemplateDataSourceId = -1;
            this.TemplateId = -1;
            this.TemplateFields = new Dictionary<string, TemplateField>();
            //this.TemplateFieldBidings = new Dictionary<string, TemplateFieldBinding>();
            //this.ProcedureCallBidings = new Dictionary<string, TemplateParameterProcedureCallBinding>();
            this.TemplateFieldBidings = new List<TemplateFieldBinding>();
            this.ProcedureCallBidings = new List<TemplateParameterProcedureCallBinding>();

        }

        public TemplateDataSource(int procedureCallId, int templateDataSourceId)
            : this()
        {
            InitializeObject(procedureCallId, templateDataSourceId);
        }

        public TemplateDataSource(int procedureCallId, int templateDataSourceId, Dictionary<string, TemplateField> templateFields)
        {
            InitializeObject(procedureCallId, templateDataSourceId);
            this.TemplateFields = templateFields;
        }

        private void InitializeObject(int procedureCallId, int templateDataSourceId)
        {
            this.ProcedureCallId = procedureCallId;
            this.TemplateDataSourceId = templateDataSourceId;
        }

        #endregion
}
}
