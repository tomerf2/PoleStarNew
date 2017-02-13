using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PoleStar.DataModel
{
    public class Sample
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "patientid")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public float Longitude { get; set; }

        [JsonProperty(PropertyName = "heartrate")]
        public int HeartRate { get; set; }
    }
}
