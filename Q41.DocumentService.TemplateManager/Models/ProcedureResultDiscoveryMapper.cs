
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Q41.DocumentService.TemplateManager.Util;

namespace Q41.DocumentService.TemplateManager.Models
{
    /// <summary>
    /// Class used to determine structure of resulting cursors from procedures. Discovery is made on DataSet returned from the procedure so this can be used on all DBMS
    /// </summary>
    public class ProcedureResultDiscoveryMapper
    {

        private Dictionary<string, ProcedureResultField> resultFields = null;
        private Dictionary<string, ProcedureParameter> procedureParamateres = null;

        private DataSet resultDs = null;

        /// <summary>
        /// Method scans the procedure result dataset and makes ResultFiledDefintion collection that is later used for mapping with template fields
        /// </summary>
        /// <param name="resultDs"></param>
        /// <param name="procedureParameters">Nedded to be able to name cursor paramaters in the resulting dictionary collection</param>
        /// <remarks>First table (cursosr) is allways individual fields. All other cursors are table cursors and are treated as a single complex field</remarks>
        public Dictionary<string, ProcedureResultField> Discover(DataSet resultDs, Dictionary<string, ProcedureParameter> parameters)
        {
            this.resultFields = new Dictionary<string, ProcedureResultField>();
            this.resultDs = resultDs;
            this.procedureParamateres = parameters;

            if (resultDs.Tables.Count == 0)
            {
                //LogManager.GetCurrentClassLogger().Fatal(ex);
                //TODO: Add specialized exception, no result in cursors
                throw new Exception("DataSet has no tables. Empty result!");
            }

            this.ReadFirstCursor();

            this.ReadTableCursors();

            return this.resultFields;


        }

        /// <summary>
        /// First cursor is allways treated as the one containing single value fields. Other cursors are treated as tabular fileds
        /// </summary>
        private void ReadFirstCursor()
        {
            foreach (DataColumn dc in this.resultDs.Tables[0].Columns)
            {
                ProcedureSimpleResultField field = new ProcedureSimpleResultField();
                field.Name = dc.ColumnName;

                this.resultFields.Add(field.Name, field);

            }
        }

        private void ReadTableCursors()
        {
            if (this.resultDs.Tables.Count <= 1)
            {
                //LogManager.GetCurrentClassLogger().Fatal(ex);
                // there are no tabular cursors
                return;
            }

            List<string> cursorNames = this.GetCursorNames();
            if (cursorNames.Count == 0 && this.resultDs.Tables.Count > 1)
            {
                // nema kursora ali iam više kursora. Ovo se radi o mssql vjerojatno, koji nema kurosre ali može vratiti više tablica.
                // uzimamo onda nazive tablice koje su se vratile u datasetu .. nema druge da stvar radi

                foreach (DataTable table in this.resultDs.Tables)
                {
                    cursorNames.Add(table.TableName);
                }
            }

            for (int i = 1; i < this.resultDs.Tables.Count; i++)
            {
                string cursorName = cursorNames[i];
                Dictionary<string, ProcedureSimpleResultField> fields = this.MapCursorFields(this.resultDs.Tables[i]);

                ProcedureComplexResultField field = new ProcedureComplexResultField();
                field.Name = cursorName;
                field.Fields = fields;

                this.resultFields.Add(cursorName, field);


            }
        }

        private Dictionary<string, ProcedureSimpleResultField> MapCursorFields(DataTable cursorResult)
        {
            Dictionary<string, ProcedureSimpleResultField> result = new Dictionary<string, ProcedureSimpleResultField>();

            foreach (DataColumn dc in cursorResult.Columns)
            {
                ProcedureSimpleResultField field = new ProcedureSimpleResultField();
                field.Name = dc.ColumnName;
                result.Add(field.Name, field);
            }

            return result;
        }

        /// <summary>
        /// Returns the list of curosr output parameters from the list of procedure parameters.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<string> GetCursorNames()
        {
            List<string> cursors = new List<string>();

            var tmp = (from x in this.procedureParamateres where x.Value.IsCursor select x.Key);
            cursors = tmp.ToList();

            return cursors;
        }
    }
}