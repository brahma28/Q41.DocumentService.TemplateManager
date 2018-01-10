using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;



namespace Q41.DocumentService.TemplateManager.Models
{
    public class DiscoverProcedureProcess
    {
        public DiscoverProcedureReq Request { get; set; }
        public DiscoverProcedureResp Response { get; set; }

        public DiscoverProcedureProcess()
        {
            this.Request = null;
            this.Response = new DiscoverProcedureResp();
        }
        //public async Task Run()
        public void Run()
        {
            if (this.Request == null)
            {
                throw new ArgumentNullException("DiscoverProcedureProcess.Request", "Procesu nisu proslijeđeni potrebni ulazni parametri");
            }
            //todo: dodati dodatne validacije i napraviti unit testove
            this.Response = new DiscoverProcedureResp();


            ProcedureDiscovery.ProcedureDiscovery discovery = new ProcedureDiscovery.ProcedureDiscovery(this.Request.Procedure);

            //await discovery.DiscoverParameters();

            //await discovery.DiscoverResult();

            this.Response.Success = true;

            this.Response.Procedure = discovery.Procedure;

        }

    }
}