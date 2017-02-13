using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class GroupCaregiver : EntityData
    {
        public string GroupID { get; set; }
        public Group Group { get; set; }
        public string CaregiverID { get; set; }
        public Caregiver Caregiver { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}