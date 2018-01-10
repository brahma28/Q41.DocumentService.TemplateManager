using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;

namespace Q41.DocumentService.TemplateManager.ProcedureDiscovery
{
    //class ProcedureParameterSqlServerDiscovery : IProcedureParameterDiscovery
    class ProcedureParameterSqlServerDiscovery 
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ProcedureCall procedure;

        //public async Task<Dictionary<string, ProcedureParameter>> DiscoverParameters(ProcedureCall procedure)
        //{
        //    this.procedure = procedure;
        //    Dictionary<string, ProcedureParameter> parameters = new Dictionary<string, ProcedureParameter>();
        //    try
        //    {
        //        parameters = await this.GetSglServerParameters();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Fatal(ex);
        //        //todo: ovdje treba javiti neki specifičan exception koji govori više o tome što se dogodilo
        //        throw;
        //    }
        //    return parameters;

        //}

        //private async Task<Dictionary<string, ProcedureParameter>> GetSglServerParameters()
        //{
        //    Dictionary<string, ProcedureParameter> rez = new Dictionary<string, ProcedureParameter>();



        //    string schema = procedure.SchemaName;
        //    string package = procedure.PackageName.Length == 0 ? "null" : "'" + procedure.PackageName + "'";
        //    string procName = procedure.ProcedureName;


        //    try
        //    {
        //        MSSQLDiscovery sqlserverDal = new MSSQLDiscovery();

        //        DataSet ds = new DataSet();
        //        //ds = await sqlserverDal.GetSqlServerProcedureParameters(this.procedure.ConnectionStringValue.Value, schema, package, procName);

        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            ProcedureParameter para = new ProcedureParameter();
        //            para.Name = dr["argument_name"].ToString();
        //            para.DataType = dr["data_type"].ToString();
        //            para.Direction = this.GetDirection(dr["in_out"].ToString());
        //            para.IsCursor = this.IsCurors(dr["data_type"].ToString());

        //            rez.Add(para.Name, para);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Fatal(ex);
        //        //todo: ovdje treba javiti neki specifičan exception koji govori više o tome što se dogodilo
        //        throw;
        //    }



        //    return rez;
        //}
        //private async Task<Dictionary<string, ProcedureParameter>> GetSqlServerParameters()
        //{
        //    Dictionary<string, ProcedureParameter> rez = new Dictionary<string, ProcedureParameter>();



        //    string schema = procedure.SchemaName;
        //    string package = procedure.PackageName.Length == 0 ? "null" : "'" + procedure.PackageName + "'";
        //    string procName = procedure.ProcedureName;


        //    try
        //    {
        //        MSSQLDiscovery mssqlDal = new MSSQLDiscovery();

        //        //DataSet ds = await mssqlDal.GetSqlServerProcedureParameters(this.procedure.ConnectionStringValue.Value, schema, package, procName);
        //        DataSet ds =  mssqlDal.GetSqlServerProcedureParameters(this.procedure.ConnectionStringValue.Value, schema, package, procName).;

        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            ProcedureParameter para = new ProcedureParameter();
        //            para.Name = dr["argument_name"].ToString();
        //            para.DataType = dr["data_type"].ToString();
        //            para.Direction = this.GetDirection(dr["in_out"].ToString());
        //            para.IsCursor = this.IsCurors(dr["data_type"].ToString());

        //            rez.Add(para.Name, para);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Fatal(ex);
        //        //todo: ovdje treba javiti neki specifičan exception koji govori više o tome što se dogodilo
        //        throw;
        //    }



        //    return rez;
        //}
        private bool IsCurors(string dataTypeName)
        {
            return dataTypeName == "REF CURSOR" ? true : false;

        }

        private Q41.DocumentService.TemplateManager.Models.ParameterDirection GetDirection(string direction)
        {
            if (direction == "IN")
            {

                return Q41.DocumentService.TemplateManager.Models.ParameterDirection.Input;
            }
            else if (direction == "OUT")
            {
                return Q41.DocumentService.TemplateManager.Models.ParameterDirection.Output;
            }
            else
            {
                return Q41.DocumentService.TemplateManager.Models.ParameterDirection.InputOutput;
            }

        }

    }
}