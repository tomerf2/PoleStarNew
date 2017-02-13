using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Group : EntityData
    {
        public string Name { get; set; }
        public string Code { get; set; }
        //public string PatientID { get; set; }
        [Required]
        public virtual Patient Patient { get; set; }

    }
}