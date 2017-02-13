using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PoleStar.DataModel
{
    public class GroupCaregiver
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "groupid")]
        public string GroupID { get; set; }

        [JsonProperty(PropertyName = "caregiverid")]
        public string CaregiverID { get; set; }

        [JsonProperty(PropertyName = "group")]
        public Group Group { get; set; }

        [JsonProperty(PropertyName = "caregiver")]
        public Caregiver Caregiver { get; set; }

        [JsonProperty(PropertyName = "lastupdated")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}
