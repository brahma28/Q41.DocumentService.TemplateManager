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
    public class TemplateField
    {
        #region Properties

        [DataMember]
        public bool IsAutoField { get; set; }
        [DataMember]
        public string Name { get; set; }

        #endregion

        #region Constructors

        public TemplateField() { }

        public TemplateField(bool isAutoField, string name)
        {
            this.IsAutoField = isAutoField;
            this.Name = name;
        }

        #endregion
    }
}
