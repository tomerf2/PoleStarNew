﻿using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Location : EntityData
    {
        public string PatientID { get; set; }
        public Patient Patient { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Description { get; set; }
        public int HeartRate { get; set; }
    }
}