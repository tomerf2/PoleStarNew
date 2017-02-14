using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
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
        public Patient()
        {
            Locations = new List<Location>();
            Samples = new List<Sample>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string GroupID { get; set; }

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        public ICollection<Location> Locations { get; set; }
        public ICollection<Sample> Samples { get; set; }
    }
}