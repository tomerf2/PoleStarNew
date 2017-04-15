using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Server.DataObjects;
using Server.Models;
using System.Collections;
using System.Collections.Generic;

namespace Server.Controllers
{
    public class PatientController : TableController<Patient>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Patient>(context, Request);
        }

        // GET tables/Patient
        public IQueryable<Patient> GetAllPatient()
        {
            return Query(); 
        }

        // GET tables/Patient/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Patient> GetPatient(string id)
        {
            return Lookup(id);
        }

        // GET a specific patient's caregivers
        public IEnumerable<Caregiver> GetCaregiversforPatientID(string patientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            Patient currentPatient = db.Patients.Where(p => p.Id == patientID).FirstOrDefault();
            var caregiversArr = db.Caregivers.Where(p => p.GroupID == currentPatient.GroupID);
            // ArrayList result = new ArrayList(caregiversArr);

            return caregiversArr;
        }

        // GET a specific patient's name
        public string GetPatientName(string patientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            string result = db.Patients.Where(p => p.Id == patientID).FirstOrDefault().Name;

            return result;
        }

        // PATCH tables/Patient/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Patient> PatchPatient(string id, Delta<Patient> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Patient
        public async Task<IHttpActionResult> PostPatient(Patient item)
        {
            Patient current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Patient/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePatient(string id)
        {
             return DeleteAsync(id);
        }
    }
}
