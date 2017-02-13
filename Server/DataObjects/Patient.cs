using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Patient : EntityData
    {
        //[ForeignKey("Group")]
        //public string GroupID { get; set; }
        public virtual Group Group { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}