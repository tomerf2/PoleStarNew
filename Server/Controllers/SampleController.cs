using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Server.DataObjects;
using Server.Models;

namespace Server.Controllers
{
    public class SampleController : TableController<Sample>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Sample>(context, Request);
        }

        // GET tables/Sample
        [EnableQuery(PageSize = 1000)]
        public IQueryable<Sample> GetAllSample()
        {
            return Query().OrderBy(s => s.CreatedAt); 
        }

        // GET tables/Sample/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Sample> GetSample(string id)
        {
            return Lookup(id);
        }

        // GET a specific patient's sample count
        public static int GetSampleCountforPatientID(string patientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            Patient currentPatient = PatientController.GetPatientObject(patientID);
            var samplesArr = db.Samples.Where(p => p.PatientID == currentPatient.Id);
            Trace.TraceInformation("Found: {0} samples for patient {1}", samplesArr.Count(), patientID);
            return samplesArr.Count();
        }

        // GET latest sample for a specific patientID
        public Sample GetLatestSampleForPatient(string patientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            Sample latestSamplForPatient = db.Samples.OrderByDescending(p => p.CreatedAt)
                .Where(p => p.PatientID == patientID).FirstOrDefault();
            return latestSamplForPatient;
        }


    // PATCH tables/Sample/48D68C86-6EA6-4C25-AA33-223FC9A27959
    public Task<Sample> PatchSample(string id, Delta<Sample> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Sample
        public async Task<IHttpActionResult> PostSample(Sample item)
        {
            Sample current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Sample/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteSample(string id)
        {
             return DeleteAsync(id);
        }
    }
}
