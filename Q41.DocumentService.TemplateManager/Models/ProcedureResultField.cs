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
    [KnownType(typeof(ProcedureSimpleResultField))]
    [KnownType(typeof(ProcedureComplexResultField))]
    public class ProcedureResultField 
    {
        #region Properties

        [DataMember]
        public string Name { get; set; }
        //public Field Fields { get; set; }
        #endregion

        #region Constructors

        public ProcedureResultField() { }

        public ProcedureResultField(string name)
        {
            this.Name = name;
        }



        #endregion

    }
}
