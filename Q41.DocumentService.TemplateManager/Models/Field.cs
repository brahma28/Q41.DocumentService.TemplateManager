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
    [KnownType(typeof(Field))]
    [KnownType(typeof(Field))]
    public class Field 
    {
        #region Properties

        [DataMember]
        public List<ProcedureResultField> KeyValueOfstringProcedureSimpleResultFieldzS_SZ5M7c { get; set; }

        #endregion

        #region Constructors

        public Field()
        {
            this.KeyValueOfstringProcedureSimpleResultFieldzS_SZ5M7c = new List<ProcedureResultField>();
        }

        #endregion

    }
}
