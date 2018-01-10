using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using Q41.DocumentService.TemplateManager.Models;


namespace Q41.DocumentService.TemplateManager.Util
{
    public class ProcedureNameGenerators 
    {
        public static string GetOracleProcedureName(ProcedureCall procedure)
        {
            string name = "";
            if (procedure.SchemaName.Length == 0)
            {
                throw new MissingSchemaException("Procedure has missing shema");
            }
            name += procedure.SchemaName + ".";

            if (procedure.PackageName.Length != 0)
            {
                name += procedure.PackageName + ".";
            }

            if (procedure.ProcedureName.Length == 0)
            {
                throw new MissingProcedureNameException("Procedure has missing name");
            }
            name += procedure.ProcedureName;

            return name;
        }

        public static string GetSqlServerProcedureName(ProcedureCall procedure)
        {
            string name = "";
            //if (procedure.SchemaName.Length == 0)
            //{
            //    throw new MissingSchemaException("Procedure has missing shema");
            //}
            //name += procedure.SchemaName + ".";

            //if (procedure.PackageName.Length != 0)
            //{
            //    name += procedure.PackageName + ".";
            //}

            if (procedure.ProcedureName.Length == 0)
            {
                throw new MissingProcedureNameException("Procedure has missing name");
            }
            name += procedure.ProcedureName;

            return name;
        }

        public static string GetDB2ServerProcedureName(ProcedureCall procedure)
        {
            string name = "";
            //if (procedure.SchemaName.Length == 0)
            //{
            //    throw new MissingSchemaException("Procedure has missing shema");
            //}
            //name += procedure.SchemaName + ".";

            //if (procedure.PackageName.Length != 0)
            //{
            //    name += procedure.PackageName + ".";
            //}

            if (procedure.ProcedureName.Length == 0)
            {
                throw new MissingProcedureNameException("Procedure has missing name");
            }
            name += procedure.ProcedureName;

            return name;
        }

    }

    public class MissingSchemaException : Exception
    {
        public MissingSchemaException(string message) : base(message) { }
    }

    public class MissingProcedureNameException : Exception
    {
        public MissingProcedureNameException(string message) : base(message) { }
    }

}
