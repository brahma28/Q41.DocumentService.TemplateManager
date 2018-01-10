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

        /// <summary>
        /// definira mapiranje template polja (TemplateField) sa ulaznim parametrima templatea (TemplateParameter) i izlaznim poljima iz proecdura (ProcedureResultField)
        /// </summary>
        [DataContract]
        public class TemplateFieldBinding
        {
            #region Properties
            /// <summary>
            /// Govori odakle dolazi vrijednost (template parametar ili rezultat procedure
            /// </summary>
            [DataMember]
            public FieldBidingSource BindingSource { get; set; }

            /// <summary>
            /// Naziv izvorišnog polja
            /// </summary>
            [DataMember]
            public string SourceName { get; set; }

            /// <summary>
            /// Naziv template fileda na koji se vrijednost mapira
            /// </summary>
            [DataMember]
            public string DestinationName { get; set; }
            #endregion

            #region Constructors

            public TemplateFieldBinding() { }
            public TemplateFieldBinding(FieldBidingSource bindingSource, string sourceName, string destinationName)
            {
                this.BindingSource = bindingSource;
                this.SourceName = sourceName;
                this.DestinationName = destinationName;
            }
            #endregion

        }

        public enum FieldBidingSource
        {
            TemplateParameter,
            ProcedureResult
        }

    }

