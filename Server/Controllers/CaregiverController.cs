using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Server.DataObjects;
using Server.Models;
using System.Diagnostics;
using System.Collections.Generic;

namespace Server.Controllers
{
    public class CaregiverController : TableController<Caregiver>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Caregiver>(context, Request);
        }

        // GET tables/Caregiver
        public IQueryable<Caregiver> GetAllCaregiver()
        {
            return Query(); 
        }

        // GET tables/Caregiver/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Caregiver> GetCaregiver(string id)
        {
            return Lookup(id);
        }


        // GET a specific patient's caregivers
        public List<Caregiver> GetCaregiversforPatientID(string patientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            Patient currentPatient = PatientController.GetPatientObject(patientID);
            //var caregiversArr = db.Caregivers.Where(p => p.GroupID == currentPatient.GroupID).AsEnumerable();
            var caregiversArr = db.Caregivers.Where(p => p.GroupID == currentPatient.GroupID);

            Trace.TraceInformation(string.Format("current patient is: {0}"), currentPatient.Id);
            Trace.Flush();
            Trace.TraceInformation(string.Format("is current patient the correct one? {0}"), currentPatient.Id == patientID);
            Trace.Flush();
            foreach (var CG in caregiversArr)
            {
                Trace.TraceInformation(string.Format("caregiver mail is: {0}"), CG.Email);
                Trace.Flush();
            }
            // ArrayList result = new ArrayList(caregiversArr);

            return caregiversArr.ToList<Caregiver>();
        }

        // PATCH tables/Caregiver/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Caregiver> PatchCaregiver(string id, Delta<Caregiver> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Caregiver
        public async Task<IHttpActionResult> PostCaregiver(Caregiver item)
        {
            Caregiver current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Caregiver/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCaregiver(string id)
        {
             return DeleteAsync(id);
        }
    }
}
