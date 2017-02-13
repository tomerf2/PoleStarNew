using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PoleStar.DataModel
{
    public class Caregiver
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "isapproved")]
        public bool IsApproved { get; set; }

        [JsonProperty(PropertyName = "groupid")]
        public string GroupID { get; set; }
    }
}
