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
    public class Group : EntityData
    {
        public Group()
        {
            Caregivers = new List<Caregiver>();
        }

        public string Name { get; set; }
        public string Code { get; set; }

        public ICollection<Caregiver> Caregivers { get; set; }
    }
}