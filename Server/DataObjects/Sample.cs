﻿using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Sample : EntityData
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int HeartRate { get; set; }

        public string PatientID { get; set; }
        [Required]
        public virtual Patient Patient { get; set; }
    }
}