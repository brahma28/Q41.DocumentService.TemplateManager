using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Q41.DocumentService.TemplateManager.Models
{
    /// <summary>
    /// veza između parametra templatea i ulaznih paramatera u ProcedureCall
    /// </summary>
    [DataContract]
    public class TemplateParameterProcedureCallBinding
    {
        #region Properties
        /// <summary>
        /// Naziv izvornog parametra predloška (TemplateParameter)
        /// </summary>
        [DataMember]
        public string TemplateParameterSource { get; set; }
        /// <summary>
        /// Naziv ulaznog parametra procedure na koji se mapira ulazni parametar predloška
        /// </summary>
        [DataMember]
        public string ProcedreCallParameterDestination { get; set; }
        #endregion

        #region Constructors
        public TemplateParameterProcedureCallBinding() { }
        public TemplateParameterProcedureCallBinding(string templateParameterSource, string procedreCallParameterDestination)
        {
            this.TemplateParameterSource = templateParameterSource;
            this.ProcedreCallParameterDestination = procedreCallParameterDestination;
        }
        #endregion
    }
}
