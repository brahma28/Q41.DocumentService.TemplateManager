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
    public class Template 
    {
        //public int Id { get; set; }1
        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public List<TemplateParameter> Parameters { get; set; }

        [DataMember]
        public TemplateDataSource DataSource { get; set; }
        //public int ProcedureCallId { get; set; }
        //public int TemplateDataSourceId { get; set; }
        //public Dictionary<string, TemplateField> TemplateFields { get; set; }
        //public Dictionary<string, TemplateParameterProcedureCallBinding> ProcedureCallBidings { get; set; }
        //public Dictionary<string, TemplateFieldBinding> TemplateFieldBidings { get; set; }

        public HttpPostedFileBase File { get; set; }

    }
}
