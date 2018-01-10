using System;
using System.Collections.Generic;
using System.Data;

using System.Data.Entity;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.IO;
using System.Text;
using Aspose.Words;
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Transactions;

using Q41.DocumentService.TemplateManager.Models;
using Q41.DocumentService.TemplateManager.Util;
using Q41.DocumentService.TemplateManager.Dal;

namespace Q41.DocumentService.TemplateManager.BAL
{
    public class Function
    {
        private Q88DocumentServiceTestEntities db = new Q88DocumentServiceTestEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string DohvatiParametarStore(string parametri)
        {
            return parametri.Substring(0, parametri.IndexOf("___"));
        }
        public static string DohvatiParametarPredloska(string parametri)
        {
            string parametarStore = "";
            string parametarPredloska = "";
            parametarStore = parametri.Substring(0, parametri.IndexOf("___"));
            parametri = parametri.Replace(parametarStore + "___", "");
            parametarPredloska = parametri.Substring(0, parametri.IndexOf("___"));
            return parametarPredloska; 
        }

        public static int DohvatiParametarTeplateId(string parametri)
        {
            
            string procedureCallId = "0";
            string templateId = "0";
            string parametarStore = "";
            string parametarPredloska = "";
            parametarStore = parametri.Substring(0, parametri.IndexOf("___"));
            parametri = parametri.Replace(parametarStore + "___", "");
            parametarPredloska = parametri.Substring(0, parametri.IndexOf("___"));
            parametri = parametri.Replace(parametarPredloska + "___", "");
            procedureCallId = parametri.Substring(0, parametri.IndexOf("___"));
            parametri = parametri.Replace(procedureCallId + "___", "");
            templateId = parametri;
            parametri = parametri.Replace(templateId, ""); // nakon ovoga bi morao ostati prazan string

            return Convert.ToInt32(templateId); 
        }
        public static int DohvatiParametarProcedureCallId(string parametri)
        {
            string procedureCallId = "0";
            //string templateId = "0";
            string parametarStore = "";
            string parametarPredloska = "";
            parametarStore = parametri.Substring(0, parametri.IndexOf("___"));
            parametri = parametri.Replace(parametarStore + "___", "");
            parametarPredloska = parametri.Substring(0, parametri.IndexOf("___"));
            parametri = parametri.Replace(parametarPredloska + "___", "");
            procedureCallId = parametri.Substring(0, parametri.IndexOf("___"));
            //parametri = parametri.Replace(procedureCallId + "___", "");
            //templateId = parametri;
            //parametri = parametri.Replace(templateId, ""); // nakon ovoga bi morao ostati prazan string
            return Convert.ToInt32(procedureCallId); 
        }

        public static string DodavanjeVezanogParametra(int procedureCallId, int templateId, string parametarStore, string parametarPredloska)
        {
            int broj = 0;
            string poruka = "";
            //string linknapredanketu = "";
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "dbo.spTemplatesParametersDodavanjeVezanogParametra";
            cmd.Parameters.Add(new SqlParameter("@ProcedureCallId", procedureCallId));
            cmd.Parameters.Add(new SqlParameter("@TemplateId", templateId));
            cmd.Parameters.Add(new SqlParameter("@ParametarTempName", parametarStore));
            cmd.Parameters.Add(new SqlParameter("@ParametarStorName", parametarPredloska));
            // izlazni parametar
            SqlParameter pvNewId = new SqlParameter();
            pvNewId.ParameterName = "@brojPovezanih";
            pvNewId.DbType = DbType.Int32;
            pvNewId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(pvNewId);
            //
            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();

                broj = Convert.ToInt32(cmd.Parameters["@brojPovezanih"].Value);
                // AKO JE BROJ > 0 => USPJEŠNO JE POVEZANO
                poruka = broj>0?"Uspješno povezano!":"Neuspješno";
                cnn.Close();
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }



            return poruka; 
        }

        public static int RegistrirajPredlozakSaStorom(int templateId, int procedureCallId, string fileName1, int templateGroupId, string paramaters, string templateFields)
        {
            int broj = 0;
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "dbo.spRegistrirajPredlozakSaStorom ";
            cmd.Parameters.Add(new SqlParameter("@TemplateId", templateId));
            cmd.Parameters.Add(new SqlParameter("@ProcedureCallId", procedureCallId));
            cmd.Parameters.Add(new SqlParameter("@Name", fileName1));
            cmd.Parameters.Add(new SqlParameter("@TemplateGroupId", templateGroupId));
            cmd.Parameters.Add(new SqlParameter("@Paramaters", paramaters));  //ViewBag.MergeFields
            cmd.Parameters.Add(new SqlParameter("@TemplateFields", templateFields));  //ViewBag.MergeFields
            // izlazni parametar
            SqlParameter newTemplateId = new SqlParameter();
            newTemplateId.ParameterName = "@NewId";
            newTemplateId.DbType = DbType.Int32;
            newTemplateId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(newTemplateId);
            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();
                cnn.Close();

            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }


            return broj; 
        }
        public static int Sinkroniziraj(int templateId, int procedureCallId) 
        {
            int broj = 0;
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            // sinkronizacija
            cmd.CommandText = "dbo.spSinkronizirajPredlozakSaStorom ";

            cmd.Parameters.Add(new SqlParameter("@TemplateId", templateId));
            cmd.Parameters.Add(new SqlParameter("@ProcedureCallId", procedureCallId));
            // izlazni parametar
            SqlParameter pvNewId = new SqlParameter();
            pvNewId.ParameterName = "@brojPovezanih";
            pvNewId.DbType = DbType.Int32;
            pvNewId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(pvNewId);
            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();
                broj = Convert.ToInt32(cmd.Parameters["@brojPovezanih"].Value);
                cnn.Close();
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return broj; 
        }
        public static int AddTemplateHeader(string fileName1, string dataDirTest, int tgId)
        {
            int templateID = 0;

            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "dbo.spAddTemplateHeader";
            cmd.Parameters.Add(new SqlParameter("@WordName", fileName1));
            cmd.Parameters.Add(new SqlParameter("@Path", dataDirTest));
            cmd.Parameters.Add(new SqlParameter("@TemplateGroupId", tgId));
            SqlParameter pvNewId = new SqlParameter();
            pvNewId.ParameterName = "@NewId";
            pvNewId.DbType = DbType.Int32;
            pvNewId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(pvNewId);
            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();
                templateID = Convert.ToInt32(cmd.Parameters["@NewId"].Value);
                cnn.Close();
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return templateID; 
        }
        public static int SpremiPredlozak(int templateId, string filename, string parametri, string dataDir) 
        {
            //int temId = Function.SpremiPredlozak(model.TemplateId, fileName1, model.Paramaters, dataDirTemp);
            int broj = 0;
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            // sinkronizacija
            cmd.CommandText = "dbo.spSpremiPredlozak ";

            cmd.Parameters.Add(new SqlParameter("@TemplateId", templateId));
            cmd.Parameters.Add(new SqlParameter("@Name", filename));
            //cmd.Parameters.Add(new SqlParameter("@Parameters", parametri));
            cmd.Parameters.Add(new SqlParameter("@Path", dataDir));

            // izlazni parametar
            SqlParameter pvNewId = new SqlParameter();
            pvNewId.ParameterName = "@brojPovezanih";
            pvNewId.DbType = DbType.Int32;
            pvNewId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(pvNewId);
            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();
                broj = Convert.ToInt32(cmd.Parameters["@brojPovezanih"].Value);
                cnn.Close();
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return broj; 
        }


        public static List<TemplateParameter> DohvatiPoljaPredloska(int templateId )
        {
            if (templateId == 0) 
            { 
                // nesto
            }
            string[] fieldNames = new string[0];
            var mergeFields = new List<TemplateParameter>();
            foreach (var mergeField in fieldNames)
            {
                mergeFields.Add(new TemplateParameter() { Name = mergeField });
            }
            return mergeFields;
        }

        public static List<ProcedureResultField> DohvatiPoljaProcedure(int procedureCallId)
        {
            if (procedureCallId == 0) 
            { 
                // nesto
            }
            string[] spFields = new string[0];
            var spProcedureFields = new List<ProcedureResultField>();
            foreach (var spField in spFields)
            {
                spProcedureFields.Add(new ProcedureResultField() { Name = spField });
            }

            return spProcedureFields; // partial view: PartialParametriProcedure.cshtml
        }
        public static List<TemplateFieldBinding> DohvatiPoljaMapiranja(int procedureCallId)
        {
            if (procedureCallId == 0) 
            { 
                // nesto
            }
            string[] spMappingFields = new string[0];

            var spBindingFields = new List<TemplateFieldBinding>();
            foreach (var spMappingField in spMappingFields)
            {
                spBindingFields.Add(new TemplateFieldBinding() { SourceName = spMappingField });
            }
            return spBindingFields;   // partial view: PartialParametriMapiranja.cshtml
        }
        public static List<TemplateFieldBinding> DohvatiPoljaMapiranjaPredloška(int templateId)
        {
            List<TemplateFieldBinding> rezultat = new List<TemplateFieldBinding>();
            if (templateId == 0) 
            { 
                // nesto
            }

            string cnnString =  System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "dbo.spTemplateDataSourcesSelectByTemplateID";
            cmd.Parameters.Add(new SqlParameter("@templateId", templateId));
            try
            {
                cnn.Open();
                var izlaz = cmd.ExecuteScalar();
                rezultat = SerializationUtil.DeserializeWithConvert<List<TemplateFieldBinding>>(izlaz.ToString());
                cnn.Close();
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return rezultat;   // partial view: PartialParametriMapiranja.cshtml
        }
        public static int ObrisiVezuMapiranjaIzmedjuPolja(int templateId, int procedureCallId, string oneProcedureField, string oneTemplateField)
        {
            int broj = 0;
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            // sinkronizacija
            cmd.CommandText = "dbo.spDeleteParamaterFieldMapping ";

            cmd.Parameters.Add(new SqlParameter("@TemplateId", templateId));
            cmd.Parameters.Add(new SqlParameter("@ProcedureCallId", procedureCallId));
            cmd.Parameters.Add(new SqlParameter("@OneProcedureField", oneProcedureField));
            cmd.Parameters.Add(new SqlParameter("@OneTemplateField", oneTemplateField));

            // izlazni parametar
            //SqlParameter pvNewId = new SqlParameter();
            //pvNewId.ParameterName = "@brojPovezanih";
            //pvNewId.DbType = DbType.Int32;
            //pvNewId.Direction = System.Data.ParameterDirection.Output;
            //cmd.Parameters.Add(pvNewId);
            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();
                //broj = Convert.ToInt32(cmd.Parameters["@brojPovezanih"].Value);
                cnn.Close();
                broj = 1; 
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return broj; 
        }

        public static List<ProcedureResultField> ListOfProcedureResultField(string[] spFields)
        {
            var spProcedureFields = new List<ProcedureResultField>();
            // izlazna polja procedure: 
            foreach (var spField in spFields)
            {
                spProcedureFields.Add(new ProcedureResultField() { Name = spField });
            }
            return spProcedureFields; 
        }

        public static List<TemplateFieldBinding> ListOfTemplateFieldBinding(string[] spMappingFields)
        {
            var spBindingFields = new List<TemplateFieldBinding>();
            // mapirani parovi: 
            foreach (var spMappingField in spMappingFields)
            {
                spBindingFields.Add(new TemplateFieldBinding() { SourceName = spMappingField });
            }
            return spBindingFields; 
        }

        public static TemplateDataSources DohvatiTemplateDataSources(int templateId)
        {
            
            //
                TemplateDataSources templatesDS = new TemplateDataSources();
                string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "dbo.spTemplateDataSourcesByTemplateId";
                cmd.Parameters.Add(new SqlParameter("@templateId", templateId));
                try
                {
                    cnn.Open();
                    var izlaz = cmd.ExecuteScalar();
                    cnn.Close();
                }
                catch (SqlException odbcEx)
                {
                    log.Fatal(odbcEx.ErrorCode);
                    throw odbcEx;
                }
                catch (System.IO.IOException ex)
                {
                    log.Fatal(ex);
                    throw ex;
                }
                finally
                {
                    if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                }
            return templatesDS;
        }

        public static string ProcedureCallUpsert(int procedureCallId, string schemaName, string packageName, string procedureName, string connectionStringName, int dbmsType, string paramaters, string resultFields)
        {
            string poruka = "";
            // conection string od baze u koju se zapisuje definicija store (web.config):
            var cnnStringQ88 = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;     // Q88 baza
            SqlConnection cnn = new SqlConnection(cnnStringQ88);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.CommandText = "dbo.spProcedureCallUpsert";
            cmd.Parameters.Add(new SqlParameter("@SchemaName", schemaName));
            cmd.Parameters.Add(new SqlParameter("@PackageName", packageName));
            cmd.Parameters.Add(new SqlParameter("@ProcedureName", procedureName));
            cmd.Parameters.Add(new SqlParameter("@ConnectionStringName", connectionStringName));
            cmd.Parameters.Add(new SqlParameter("@DbmsType", dbmsType));
            cmd.Parameters.Add(new SqlParameter("@Paramaters", paramaters));
            cmd.Parameters.Add(new SqlParameter("@ResultFields", resultFields));
            // izlazni parametar
            SqlParameter pvNewId = new SqlParameter();
            pvNewId.ParameterName = "@ProcedureCallId";
            pvNewId.DbType = DbType.Int32;
            pvNewId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(pvNewId);

            try
            {
                cnn.Open();
                object o = cmd.ExecuteScalar();
                procedureCallId = Convert.ToInt32(cmd.Parameters["@ProcedureCallId"].Value);
                cnn.Close();
                poruka = "Procedura: '" + procedureName + "', je uspješno spremljena.";
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                if (odbcEx.ErrorCode == -2146232060)
                {
                    poruka = "<script language='javascript' type='text/javascript'>alert('" + odbcEx.Message + "');</script>";
                    
                }
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }
            // formatiranje XML-a
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = cnn;
            cmd2.CommandType = System.Data.CommandType.StoredProcedure;
            cmd2.CommandText = "dbo.spFormatProcedureResultField";
            cmd2.Parameters.Add(new SqlParameter("@ProcedureCallId", procedureCallId));
            try
            {
                cnn.Open();
                object o = cmd2.ExecuteScalar();
                cnn.Close();
                poruka = "Proceduri: '" + procedureName + "', je uspješno formatiran XML.";
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                if (odbcEx.ErrorCode == -2146232060)
                {
                    poruka = "<script language='javascript' type='text/javascript'>alert('" + odbcEx.Message + "');</script>";

                }
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }


            return poruka; 
        }

        public static string ObrisiParametarProcedure(int procedureCallId, string parametar)
        {
            string poruka = "";
            // poziv procedure koja brise parametar
            // [spProcedureCallsBrisanjeParametraById] @procedureCallId, @Parametar
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            try
            {
                cmd.CommandText = "dbo.spProcedureCallsBrisanjeParametraById";
                cmd.Parameters.Add(new SqlParameter("@procedureCallId", procedureCallId));
                cmd.Parameters.Add(new SqlParameter("@Parametar", parametar));
                cnn.Open();
                object o = cmd.ExecuteScalar();
                //templateID = Convert.ToInt32(cmd.Parameters["@NewId"].Value);
                //model.TemplateId = Convert.ToInt32(cmd.Parameters["@NewId"].Value);
                cnn.Close();
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                log.Fatal(odbcEx.Message);
                if (odbcEx.ErrorCode == -2146232060)
                {
                    poruka = "<script language='javascript' type='text/javascript'> alert('" + odbcEx.Message + "'); </script>";

                }
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return poruka; 
        
        }

        public static string DodajNovoPolje(int procedureCallId, string parametar, string cursor)
        {
            string poruka = "";
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DokumentServisTest"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            try
            {
                cmd.CommandText = "dbo.spProcedureCallsDodavanjeParametraById";
                cmd.Parameters.Add(new SqlParameter("@procedureCallId", procedureCallId));
                cmd.Parameters.Add(new SqlParameter("@Parametar", parametar));
                cmd.Parameters.Add(new SqlParameter("@Cursor", cursor));
                cnn.Open();
                object o = cmd.ExecuteScalar();
                cnn.Close();
                
            }
            catch (SqlException odbcEx)
            {
                log.Fatal(odbcEx.ErrorCode);
                if (odbcEx.ErrorCode == -2146232060)
                {
                    poruka = "<script language='javascript' type='text/javascript'>alert('" + odbcEx.Message + "');</script>";
                    //return Content(poruka);
                }
                throw odbcEx;
            }
            catch (System.IO.IOException ex)
            {
                log.Fatal(ex);
                throw ex;
            }
            finally
            {
                if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
            }

            return poruka; 
        
        }

    }
}