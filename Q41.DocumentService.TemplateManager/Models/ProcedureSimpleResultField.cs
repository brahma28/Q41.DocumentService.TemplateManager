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
    public class ProcedureSimpleResultField : ProcedureResultField
    {
        #region Contstructors

        public ProcedureSimpleResultField() { }

        public ProcedureSimpleResultField(string name) : base(name) { }

        #endregion
    }
}