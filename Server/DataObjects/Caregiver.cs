using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public virtual Group Group { get; set; }
    }
}