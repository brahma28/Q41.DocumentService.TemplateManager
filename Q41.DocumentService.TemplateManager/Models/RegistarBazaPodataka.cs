using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Q41.DocumentService.TemplateManager.Models
{
    public class RegistarBazaPodataka
    {

        public RegistarBazaPodataka()
        {
            ActionsList = new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> ActionsList { get; set; }

        public string TipBaze { get; set; }

    }

}