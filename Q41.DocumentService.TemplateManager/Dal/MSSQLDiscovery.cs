using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.ProcedureDiscovery;
using System.Data.SqlClient;
//using Oracle.DataAccess.Client;

namespace Q41.DocumentService.TemplateManager.Dal
{
    /// <summary>
    /// Pozivi prema MSSQLServer bazi za potrebe Discovery-a procedura
    /// // da li treba TOP 1 ili ne
    /// </summary>
    public class MSSQLDiscovery
    {
        private static readonly log4net.ILog log =
         log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Dictionary<string, ProcedureInputParameter> DohvatiUlazneParametreStoreDictionary(string connectionString, string schema, string package, string procedureName)
        {
            Dictionary<string, ProcedureInputParameter> dic = new Dictionary<string, ProcedureInputParameter>();
            List<ProcedureParameter> izlaz = new List<ProcedureParameter>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string commandText = String.Format(
                    "SELECT PARAMETER_NAME as Name, PARAMETER_MODE as in_out, DATA_TYPE as DataType, " +
                    "CASE WHEN PARAMETER_MODE = 'IN' THEN 'Input' WHEN PARAMETER_MODE = 'INOUT' THEN 'InputOutput' WHEN PARAMETER_MODE = 'OUT' THEN 'Output'ELSE 'ReturnValue' END as Direction " +
                    "FROM INFORMATION_SCHEMA.PARAMETERS " +
                    "WHERE SPECIFIC_NAME='{2}' order by ORDINAL_POSITION", schema, package, procedureName);

                SqlCommand cmd = new SqlCommand(commandText, conn);
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);
                }
                catch (SqlException odbcEx)
                {
                    switch (odbcEx.State)
                    {
                        case 250:
                            var gres1 = odbcEx.ErrorCode;
                            log.Fatal(gres1);
                            // podici gresku prema korisniku
                            break;
                        case 251:
                            var gres2 = odbcEx.ErrorCode;
                            log.Fatal(gres2);
                            // podici gresku prema korisniku
                            break;
                        default:
                            break;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    log.Fatal(ex);
                }
                finally
                {
                    //if (sqlConnection != null) { sqlConnection.Close(); }
                }

                var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new ProcedureInputParameter { Name = dataRow.Field<string>("Name"), DataType = dataRow.Field<string>("DataType"), IsCursor = false }).ToList();
                //var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new Dictionary<string, ProcedureResultField> { TKey = dataRow.Field<string>("Name") }).ToList();

                foreach (ProcedureInputParameter s in popis)
                {
                    dic.Add(s.Name, s);
                }
                //var res = popis.ToDictionary(x => x, x => string.Format("Key: {0}", x));

            }
            return dic;
        }

        public static Dictionary<string, ProcedureResultField> DohvatiResultFieldParametreStoreDictionary(string connectionString, string schema, string package, string procedureName)
        {

            Dictionary<string, ProcedureResultField> dic = new Dictionary<string, ProcedureResultField>();
            ProcedureResultField prf = new ProcedureResultField(); 
            List<string> paramNames = new List<string>();

            try
            {
                //paramNames = GetProcedureParameters(connectionString, schema, package, procedureName);
                paramNames = GetProcedureParametersEXEC(connectionString, schema, package, procedureName);
                dic = GetProcedureParameters2Dic(connectionString, schema, package, procedureName);
            }
            catch (Exception ex)
            {
                var greska = "Neuspio pokušaj čitanja store kroz execute sa null ulazom." + ex.Message.ToString();
                log.Fatal(greska); 
            }

            if (paramNames.Count > 0)
            {
                return dic;    
            }


            List<ProcedureResultField> izlaz = new List<ProcedureResultField>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string commandText = String.Format("SELECT name as Name, system_type_name as DataType FROM sys.dm_exec_describe_first_result_set_for_object(OBJECT_ID('" + procedureName + "'), NULL)");
                SqlCommand cmd = new SqlCommand(commandText, conn);
                try
                {
                    conn.Open();
                }
                catch(Exception ex)
                {
                    var greska = "Neuspio pokušaj spajanja na bazu! (DohvatiResultFieldParametreStoreDictionary) connectionString: " + connectionString + ", " + ex.Message.ToString(); 
                    log.Fatal(greska);                    
                    //throw;
                    return dic;
                }
                finally
                {
                    //if (sqlConnection != null) { sqlConnection.Close(); }
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);
                }
                catch (SqlException odbcEx)
                {
                    switch (odbcEx.State)
                    {
                        case 250:
                            var gres1 = odbcEx.ErrorCode;
                            log.Fatal(gres1);
                            // podici gresku prema korisniku
                            break;
                        case 251:
                            var gres2 = odbcEx.ErrorCode;
                            log.Fatal(gres2);
                            // podici gresku prema korisniku
                            break;
                        default:
                            break;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    log.Fatal(ex);
                }
                finally
                {
                    //if (sqlConnection != null) { sqlConnection.Close(); }
                }
                var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new ProcedureResultField { Name = dataRow.Field<string>("Name") }).ToList();
                //var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new Dictionary<string, ProcedureResultField> { TKey = dataRow.Field<string>("Name") }).ToList();
               
                foreach (ProcedureResultField s in popis)
                {
                    if (String.IsNullOrEmpty(s.Name))
                    {
                        // eventualno napisati info
                    }
                    else
                    { 
                        dic.Add(s.Name, s);                    
                    }

                }
                //var res = popis.ToDictionary(x => x, x => string.Format("Key: {0}", x));

            }
            return dic;
        }

        private static List<string> GetProcedureParameters(string connectionString, string schema, string package, string procedureName) {

            //Dictionary<string, ProcedureInputParameter> inputParamsDesc = DohvatiUlazneParametreStoreDictionary(connectionString, schema, package, procedureName);
            //ProcedureInputParameter inputParam = inputParamsDesc.First().Value;

            using (SqlConnection conn = new SqlConnection(connectionString)) {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.CommandText = schema + "." + procedureName;
                //cmd.Parameters.Add(new SqlParameter(inputParam.Name, null));

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format("EXEC {0}.{1} null", schema, procedureName);

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables == null || ds.Tables.Count == 0) {
                    return new List<string>();
                }

                List<string> paramNames = new List<string>();

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (i == 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dc.ColumnName);
                        }
                    }
                    else
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dt.TableName + "." + dc.ColumnName);
                        }
                    }
                }

                return paramNames;
            }
        }
        private static string DohvatiExecString(string connectionString, string schema, string package, string procedureName)
        {
            string execString = String.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string commandText = String.Format(
                    "SELECT PARAMETER_NAME as Name, PARAMETER_MODE as in_out, DATA_TYPE as DataType, " +
                    "CASE WHEN PARAMETER_MODE = 'IN' THEN 'Input' WHEN PARAMETER_MODE = 'INOUT' THEN 'InputOutput' WHEN PARAMETER_MODE = 'OUT' THEN 'Output'ELSE 'ReturnValue' END as Direction " +
                    "FROM INFORMATION_SCHEMA.PARAMETERS " +
                    "WHERE SPECIFIC_NAME='{2}' order by ORDINAL_POSITION", schema, package, procedureName);

                SqlCommand cmd = new SqlCommand(commandText, conn);
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);
                }
                catch (SqlException odbcEx)
                {
                    switch (odbcEx.State)
                    {
                        case 250:
                            var gres1 = odbcEx.ErrorCode;
                            log.Fatal(gres1);
                            // podici gresku prema korisniku
                            break;
                        case 251:
                            var gres2 = odbcEx.ErrorCode;
                            log.Fatal(gres2);
                            // podici gresku prema korisniku
                            break;
                        default:
                            break;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    log.Fatal(ex);
                }
                finally
                {
                    if (conn != null) { conn.Close(); }
                }

                var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new ProcedureInputParameter { Name = dataRow.Field<string>("Name"), DataType = dataRow.Field<string>("DataType"), IsCursor = false }).ToList();

                foreach (ProcedureInputParameter s in popis)
                {
                    if (s.DataType == "int" || s.DataType == "bigint" || s.DataType == "tinyint" || s.DataType == "smallint" || s.DataType == "numeric" )
                    {
                        execString = (execString == "") ? "1 " : execString + ", 1 "; 
                    }
                    else if (s.DataType == "float" || s.DataType == "real" ) 
                    { 
                        execString = (execString == "") ? "null " : execString + ", null "; // 1.18E - 38
                    }
                    else if (s.DataType == "money" || s.DataType == "smallmoney" )
                    {
                        execString = (execString == "") ? "0.00 " : execString + ", 0.00 ";           
                    }
                    else if (s.DataType == "time" )
                    {
                        execString = (execString == "") ? "'00:00:00' " : execString + ", '00:00:00' ";          
                    }
                    else if (s.DataType == "date" )
                    {
                        execString = (execString == "") ? "'1900-01-01' " : execString + ", '1900-01-01' ";          
                    }
                    else if (s.DataType == "datetime" || s.DataType == "datetime2" || s.DataType == "smalldatetime" || s.DataType == "time")
                    {
                        execString = (execString == "") ? "'1900-01-01 00:00:00' " : execString + ", '1900-01-01 00:00:00' ";          
                    }
                    else if (s.DataType == "datetimeoffset")
                    {
                        execString = (execString == "") ? "'1900-01-01 00:00:00 00:00' " : execString + ", '1900-01-01 00:00:00 00:00' ";          
                    }
                    else if (s.DataType == "nvarchar" || s.DataType == "varchar" || s.DataType == "nchar"  || s.DataType == "char" || s.DataType == "ntext" || s.DataType == "text")
                    { 
                        if (execString == "") 
                        {
                            execString = "N'' ";       
                        } 
                        else 
                        {
                            execString = execString + ", N'' ";                            
                        }            
                    }
                    else 
                    {   // image, binary, varbinary, OTHER...
                        if (execString == "") 
                        {
                            execString = "null ";       
                        } 
                        else 
                        {
                            execString = execString + ", null ";                            
                        }            
                    }
                }
                //var res = popis.ToDictionary(x => x, x => string.Format("Key: {0}", x));
            }
            return execString; 
        }
        private static List<string> GetProcedureParametersEXEC(string connectionString, string schema, string package, string procedureName)
        {

            string execString = null;
            execString = DohvatiExecString(connectionString, schema, package, procedureName);
            execString = "EXEC " + schema + "." + procedureName + " " + execString ;
            //execString = String.Format("EXEC {0}.{1} {3}", schema, procedureName, execString);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = execString;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables == null || ds.Tables.Count == 0)
                {
                    return new List<string>();
                }

                List<string> paramNames = new List<string>();

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (i == 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dc.ColumnName);
                        }
                    }
                    else
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dt.TableName + "." + dc.ColumnName);
                        }
                    }
                }

                return paramNames;
            }
        }
        private static Dictionary<string, ProcedureResultField> GetProcedureParameters2Dic(string connectionString, string schema, string package, string procedureName)
        {
            Dictionary<string, ProcedureResultField> rezultat = new Dictionary<string, ProcedureResultField>();

            string execString = null;
            execString = DohvatiExecString(connectionString, schema, package, procedureName);
            //Dictionary<string, ProcedureInputParameter> inputParamsDesc = DohvatiUlazneParametreStoreDictionary(connectionString, schema, package, procedureName);
            //ProcedureInputParameter inputParam = inputParamsDesc.First().Value;

            using (SqlConnection conn = new SqlConnection(connectionString)) {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.CommandText = schema + "." + procedureName;
                //cmd.Parameters.Add(new SqlParameter(inputParam.Name, null));

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format("EXEC {0}.{1} null", schema, procedureName);

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables == null || ds.Tables.Count == 0) {
                    return new Dictionary<string, ProcedureResultField>();
                }

                List<string> paramNames = new List<string>();

                Dictionary<string, ProcedureResultField> sprfDic = new Dictionary<string, ProcedureResultField>(); 
                Dictionary<string, ProcedureResultField> sprfDic0 = new Dictionary<string, ProcedureResultField>(); 
                Dictionary<string, ProcedureResultField> sprfDic1 = new Dictionary<string, ProcedureResultField>(); 


                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (i == 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dc.ColumnName);

                            ProcedureResultField prf0 = new ProcedureResultField();
                            prf0.Name = dc.ColumnName;
                            sprfDic0.Add(dc.ColumnName, prf0); 
                        }
                    }
                    else
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dt.TableName + "." + dc.ColumnName);
                            ProcedureResultField prf1 = new ProcedureResultField();
                            prf1.Name = dt.TableName + "." + dc.ColumnName;
                            sprfDic1.Add(dt.TableName + "." + dc.ColumnName, prf1); 
                        }
                    }
                }

                foreach (var element in sprfDic1) {
                    sprfDic0.Add(element.Key, element.Value);               
                }


                //return paramNames;
                return sprfDic0;
            }
        }
        private static Dictionary<string, ProcedureResultField> GetProcedureParameters2ComplexDic(string connectionString, string schema, string package, string procedureName)
        {
            Dictionary<string, ProcedureResultField> rezultat = new Dictionary<string, ProcedureResultField>();


            string execString = null;
            execString = DohvatiExecString(connectionString, schema, package, procedureName);
            //Dictionary<string, ProcedureInputParameter> inputParamsDesc = DohvatiUlazneParametreStoreDictionary(connectionString, schema, package, procedureName);
            //ProcedureInputParameter inputParam = inputParamsDesc.First().Value;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.CommandText = schema + "." + procedureName;
                //cmd.Parameters.Add(new SqlParameter(inputParam.Name, null));

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format("EXEC {0}.{1} null", schema, procedureName);

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables == null || ds.Tables.Count == 0)
                {
                    return new Dictionary<string, ProcedureResultField>();
                }

                List<string> paramNames = new List<string>();

                Dictionary<string, ProcedureResultField> sprfDic = new Dictionary<string, ProcedureResultField>();
                Dictionary<string, ProcedureResultField> sprfDic0 = new Dictionary<string, ProcedureResultField>();
                Dictionary<string, ProcedureResultField> sprfDic1 = new Dictionary<string, ProcedureResultField>();
            //Dictionary<string, ProcedureComplexResultField> rezultatComplex = new Dictionary<string, ProcedureComplexResultField>();
            //ProcedureComplexResultField pcrf = new ProcedureComplexResultField();

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (i == 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dc.ColumnName);

                            ProcedureResultField prf0 = new ProcedureResultField();
                            prf0.Name = dc.ColumnName;
                            sprfDic0.Add(dc.ColumnName, prf0);


                        }
                    }
                    else
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            paramNames.Add(dt.TableName + "." + dc.ColumnName);
                            ProcedureResultField prf1 = new ProcedureResultField();
                            prf1.Name = dt.TableName + "." + dc.ColumnName;
                            sprfDic1.Add(dt.TableName + "." + dc.ColumnName, prf1);
                        }
                    }
                }

                foreach (var element in sprfDic1)
                {
                    sprfDic0.Add(element.Key, element.Value);
                }


                //return paramNames;
                return sprfDic0;
            }
        }

        public static List<ProcedureResultField> DohvatiResultFieldParametreStore(string connectionString, string schema, string package, string procedureName)
        {
            List<ProcedureResultField> izlaz = new List<ProcedureResultField>(); 
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string commandText = String.Format("SELECT name as Name, system_type_name as DataType FROM sys.dm_exec_describe_first_result_set_for_object(OBJECT_ID('" + procedureName + "'), NULL)");
                SqlCommand cmd = new SqlCommand(commandText, conn);
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);
                }
                catch (SqlException odbcEx)
                {
                    switch (odbcEx.State)
                    {
                        case 250:
                            var gres1 = odbcEx.ErrorCode;
                            log.Fatal(gres1);
                            // podici gresku prema korisniku
                            break;
                        case 251:
                            var gres2 = odbcEx.ErrorCode;
                            log.Fatal(gres2);
                            // podici gresku prema korisniku
                            break;
                        default:
                            break;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    log.Fatal(ex);
                }
                finally
                {
                    //if (sqlConnection != null) { sqlConnection.Close(); }
                }
                var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new ProcedureResultField { Name = dataRow.Field<string>("Name")}).ToList();
                //var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new Dictionary<string, ProcedureResultField> { TKey = dataRow.Field<string>("Name") }).ToList();
                Dictionary<string, ProcedureResultField> dic = new Dictionary<string, ProcedureResultField>();
                foreach (ProcedureResultField s in popis)
                {
                    dic.Add(s.Name, s);
                }

                var iii = dic; 
                var res = popis.ToDictionary(x => x, x => string.Format("Key: {0}", x));
                izlaz = popis; 
            }


            return izlaz; 
        }

        public static List<ProcedureParameter> DohvatiParametreStore(string connectionString, string schema, string package, string procedureName){
            List<ProcedureParameter> izlaz = new List<ProcedureParameter>(); 
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string commandText = String.Format("SELECT name as Name, system_type_name as DataType FROM sys.dm_exec_describe_first_result_set_for_object(OBJECT_ID('" + procedureName + "'), NULL)");
                SqlCommand cmd = new SqlCommand(commandText, conn);
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);
                }
                catch (SqlException odbcEx)
                {
                    switch (odbcEx.State)
                    {
                        case 250:
                            var gres1 = odbcEx.ErrorCode;
                            log.Fatal(gres1);
                            // podici gresku prema korisniku
                            break;
                        case 251:
                            var gres2 = odbcEx.ErrorCode;
                            log.Fatal(gres2);
                            // podici gresku prema korisniku
                            break;
                        default:
                            break;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    log.Fatal(ex);
                }
                finally
                {
                    //if (sqlConnection != null) { sqlConnection.Close(); }
                }
                var popis = ds.Tables[0].AsEnumerable().Select(dataRow => new ProcedureParameter { Name = dataRow.Field<string>("Name"), DataType = dataRow.Field<string>("DataType") }).ToList();
                izlaz = popis; 
            }


            return izlaz; 
        }


        public  Task<DataSet> GetSqlServerProcedureParameters(string connectionString, string schema, string package, string procedureName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string commandText = String.Format(
                        "SELECT PARAMETER_NAME as argument_name, PARAMETER_MODE as in_out, DATA_TYPE as data_type FROM INFORMATION_SCHEMA.PARAMETERS " +
                        "WHERE SPECIFIC_NAME='{2}' order by ORDINAL_POSITION", schema, package, procedureName);
                    SqlCommand cmd = new SqlCommand(commandText, conn);


                    //await conn.OpenAsync();
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    try
                    {
                        //todo: Pogledati kako ovo napraviti async ... SqlDataReader ima async verzije ....
                        da.Fill(ds);
                    }
                    catch (SqlException odbcEx)
                    {
                        switch (odbcEx.State)
                        {
                            case 250:
                                var gres1 = odbcEx.ErrorCode;
                                log.Fatal(gres1);
                                // podici gresku prema korisniku
                                break;
                            case 251:
                                var gres2 = odbcEx.ErrorCode;
                                log.Fatal(gres2);
                                // podici gresku prema korisniku
                                break;
                            default:
                                break;
                        }
                    }
                    catch (System.IO.IOException ex)
                    {
                        log.Fatal(ex);
                    }
                    finally
                    {
                        //if (sqlConnection != null) { sqlConnection.Close(); }
                    }
                    //return ds;
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                throw;
            }
        }


        /// <summary>
        /// Izvršava generički poziv neke mssql server procedure. Koristi se tokom procesa otkrivanja izlaza iz procedure (ProcedureDiscovery proces)
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        //public async Task<DataSet> GenericProcedureCall(ProcedureCall procedure, Dictionary<string, object> values)
        public static DataSet GenericProcedureCall(ProcedureCall procedure, Dictionary<string, object> values)
        {
            SqlConnection conn = new SqlConnection(procedure.ConnectionStringValue.Value);
            SqlCommand cmd = CreateCommand(procedure, values);
            DataSet ds = new DataSet();

            try
            {
                //await conn.OpenAsync();
                conn.Open();
                cmd.Connection = conn;

                //todo: istražiti kako i ovo da bude async ... OracleDataReader ima async verziju
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);


            }
            catch (SqlException odbcEx)
            {
                switch (odbcEx.State)
                {
                    case 250:
                        var gres1 = odbcEx.ErrorCode;
                        log.Fatal(gres1);
                        // podici gresku prema korisniku
                        break;
                    case 251:
                        var gres2 = odbcEx.ErrorCode;
                        log.Fatal(gres2);
                        // podici gresku prema korisniku
                        break;
                    default:
                        break;
                }
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw;
            }
            catch (Exception e)
            {
                log.Fatal(e);
                throw;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed) conn.Close();
            }
            //return null;
            return ds;

        }


        /// <summary>
        /// Creates OracleCommand from discovered parameter list
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static SqlCommand CreateCommand(ProcedureCall procedure, Dictionary<string, object> values)
        {
            SqlCommand cmd = new SqlCommand(ProcedureNameGenerators.GetSqlServerProcedureName(procedure));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;


            int numParams = 0;

            // if the procedure has no defined paramaters or has no input parameters throw an exception??
            // go trough procedures parameters and find corelating value in values collection

            foreach (KeyValuePair<string, ProcedureParameter> para in procedure.Paramaters)
            {
                numParams++;
                if (para.Value.Direction == Q41.DocumentService.TemplateManager.Models.ParameterDirection.Input || para.Value.Direction == Q41.DocumentService.TemplateManager.Models.ParameterDirection.InputOutput)
                {

                    string paraName = para.Value.Name;

                    object paraValue = null;
                    if (values == null || !values.ContainsKey(para.Value.Name) || values[para.Value.Name] == null)
                    {
                        paraValue = DBNull.Value;
                    }
                    else
                    {
                        paraValue = values[para.Value.Name];
                    }

                    SqlParameter p = cmd.Parameters.Add(paraName, paraValue);

                }
                else
                {
                    SqlParameter sqlPara = new SqlParameter();
                    sqlPara.ParameterName = para.Value.Name;
                    sqlPara.Direction = System.Data.ParameterDirection.Output;

                    if (para.Value.DataType == "REF CURSOR")
                    {
                        //to do MEDVED: ne znam sto je sa cursorom u ms sql serveru ?????
                        //sqlPara.SqlDbType = SqlDbType.RefCursor;
                    }
                    cmd.Parameters.Add(sqlPara);

                }

            }

            if (numParams == 0)
            {
                // da li je ovo potrebno? Moguće se procedure bez ikakvih parametara?? Na oracleu mozda i ne zbog izlaznih kursora
                throw new Exception("No parameters were discovered");
            }

            return cmd;
        }

        public static bool TestConnection(string connectionString)
        {
            bool proba = false;
            if (connectionString != String.Empty)
            { 
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string commandText = String.Format("");
                    SqlCommand cmd = new SqlCommand(commandText, conn);
                    try
                    {
                        conn.Open();
                        proba = true; 
                    }
                    catch(Exception ex)
                    {
                        var greska = "Neuspio pokušaj spajanja na bazu! connectionString: " + connectionString + ", " + ex.Message.ToString(); 
                        log.Fatal(greska);                    
                    }
                    finally
                    {
                        if (conn != null) { conn.Close(); }
                    }
                }            
            }


            return proba; 
        
        }






    }
}