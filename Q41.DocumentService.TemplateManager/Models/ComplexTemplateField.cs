using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Q41.DocumentService.TemplateManager.Models
{
    [DataContract]
    /// <summary>
    /// definira komplkesno teplate polje. Konkretno, radi se o poljima koji su tabličnog formata .. tj sadrže više istih redova. Ovo se koristi kada se u rezultatu proceure nalaze ProcedureComplexResultField polja koja se mapiraju na ovo polje
    /// </summary>
    public class ComplexTemplateField : TemplateField
    {
        [DataMember]
        /// <summary>
        /// Djeca kompleksnog polja
        /// </summary>
        public List<TemplateField> Fields { get; set; }
    }
}