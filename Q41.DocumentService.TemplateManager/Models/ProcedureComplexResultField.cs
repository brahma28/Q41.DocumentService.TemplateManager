using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;



namespace Q41.DocumentService.TemplateManager.Models
{
    [DataContract]
    public class ProcedureComplexResultField : ProcedureResultField
    {
        #region Properties

        [DataMember]
        public Dictionary<string, ProcedureSimpleResultField> Fields { get; set; }

        #endregion

        #region Constructors

        public ProcedureComplexResultField()
        {
            this.Fields = new Dictionary<string, ProcedureSimpleResultField>();
        }

        public ProcedureComplexResultField(string name)
            : this()
        {
            InitializeObject(name);
        }

        public ProcedureComplexResultField(string name, Dictionary<string, ProcedureSimpleResultField> fields)
        {
            InitializeObject(name);
            this.Fields = fields;
        }

        private void InitializeObject(string name)
        {
            base.Name = name;
        }

        #endregion
    }
}