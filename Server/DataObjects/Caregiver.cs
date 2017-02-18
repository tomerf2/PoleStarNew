using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Caregiver : EntityData
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsApproved { get; set; }

        public string GroupID { get; set; }
        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }
    }
}