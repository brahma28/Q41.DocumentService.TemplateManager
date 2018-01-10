using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Q41.DocumentService.TemplateManager.Models;
using System.Threading.Tasks;

namespace Q41.DocumentService.TemplateManager.Util
{
    public interface IProcedureResultDiscovery
    {
        //todo: ovdje fali način da se prosliejde ulazni podaci za parametre iz vana
        Dictionary<string, ProcedureResultField> Discover(ProcedureCall procedure, Dictionary<string, object> values);
    }
}