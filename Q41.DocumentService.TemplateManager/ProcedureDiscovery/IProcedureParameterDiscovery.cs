using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;

namespace Q41.DocumentService.TemplateManager.ProcedureDiscovery
{
    public interface IProcedureParameterDiscovery
    {
        Dictionary<string, ProcedureParameter> DiscoverParameters(ProcedureCall procedure);
    }
}