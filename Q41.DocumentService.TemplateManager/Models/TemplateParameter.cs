using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace Q41.DocumentService.TemplateManager.Models
{
    public class TemplateParameter
    {
        #region Properties

        public TemplateParameterDataType DataType { get; set; }
        public bool IsRequired { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }

        #endregion

        #region Constructors

        public TemplateParameter() { }

        public TemplateParameter(TemplateParameterDataType dataType, bool isRequired, string name, string opis, string primjer)
        {
            this.DataType = dataType;
            this.IsRequired = isRequired;
            this.Name = name;
            this.Description = opis;
            this.Example = primjer;
        }

        #endregion
    }
}
