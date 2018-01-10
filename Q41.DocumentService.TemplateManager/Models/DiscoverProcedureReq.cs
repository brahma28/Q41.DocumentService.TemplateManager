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
    public class DiscoverProcedureReq
    {
        [DataMember]
        public ProcedureCall Procedure { get; set; }

    }
}