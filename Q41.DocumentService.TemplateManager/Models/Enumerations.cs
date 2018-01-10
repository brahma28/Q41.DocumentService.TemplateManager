using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Q41.DocumentService.TemplateManager.Models
{
    public enum DbmsType
    {
        Db2zOS = 1,
        Oracle = 2,
        SqlServer = 3,
        Unknown = 4
    }

    public enum ParameterDirection
    {
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 4
    }

    public enum ParameterBindingDestination
    {
        StoredProcedure = 1,
        TemplateField = 2
    }

    public enum TemplateParameterDataType
    {
        integer = 1,
        nvarchar = 2, // <- u predlošku su svi stringovi
        datetime = 3,
        bit = 4,
        xml = 5,
        hierarchyid = 6 // <- ovo treba koristiti za hijerarhiju grupa
    }

    public static class Enumerations
    {
        #region Methods

        public static DbmsType ConvertIntToDbmsType(int value)
        {
            DbmsType returnValue = DbmsType.Unknown;

            switch (value)
            {
                case 1:
                    returnValue = DbmsType.Db2zOS;
                    break;
                case 2:
                    returnValue = DbmsType.Oracle;
                    break;
                case 3:
                    returnValue = DbmsType.SqlServer;
                    break;
            }

            return returnValue;
        }

        #endregion
    }
}
