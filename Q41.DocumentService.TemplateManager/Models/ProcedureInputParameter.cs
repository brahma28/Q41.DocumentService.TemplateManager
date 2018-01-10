using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace Q41.DocumentService.TemplateManager.Models
{
    public class ProcedureInputParameter
    {

        #region Properties

        public string DataType { get; set; }
        public bool IsCursor { get; set; }
        public string Name { get; set; }

        #endregion

        #region Constructors

        public ProcedureInputParameter() { }

        public ProcedureInputParameter(string dataType, bool isCursor, string name)
        {
            this.DataType = dataType;
            this.IsCursor = isCursor;
            this.Name = name;
        }

        #endregion
    }
}
