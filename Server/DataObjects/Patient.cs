using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DataObjects
{
    public class Patient : EntityData
    {
        public string GroupID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}