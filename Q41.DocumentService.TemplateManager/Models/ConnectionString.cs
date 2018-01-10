using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Q41.DocumentService.TemplateManager.Models
{
    public class ConnectionString 
    {
        #region Properties

        public string Name { get; set; }
        public string Value { get; set; }
        public DbmsType DbmsType { get; set; }

        #endregion

        #region Constructors

        public ConnectionString() { }

        public ConnectionString(string name, string value, DbmsType dbmsType)
        {
            this.Name = name;
            this.Value = value;
            this.DbmsType = dbmsType;
        }

        #endregion
    }
}
