using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Server.Models;
using Server.DataObjects;
using System.Web.Http.OData;
using System.Linq;

namespace Server.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    [RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        [HttpGet]
        [Route("GetAllSamplesByPatientID")]
        [EnableQuery(PageSize = 1000)]
        public IQueryable<Sample> GetAllSamplesByPatientID(string patientID)
        {
            MobileServiceContext context = new MobileServiceContext();

            return context.Samples.Where(s => s.PatientID == patientID).AsQueryable<Sample>();
        }

        // GET api/values
        /*public string Get()
        {
            return "Hello World!";
        }

        // POST api/values
        public string Post()
        {
            return "Hello World!";
        }*/
    }
}
