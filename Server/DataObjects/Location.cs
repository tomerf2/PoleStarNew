using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Location : EntityData
    {
        public string CaregiverID { get; set; }
        public string PatientID { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Description { get; set; }
    }
}