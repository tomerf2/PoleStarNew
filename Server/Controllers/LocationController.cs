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
    public class LocationController : TableController<Location>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Location>(context, Request);
        }

        // GET tables/Location
        public IQueryable<Location> GetAllLocation()
        {
            return Query(); 
        }

        // GET tables/Location/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Location> GetLocation(string id)
        {
            return Lookup(id);
        }


        // GET tables/Location/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Location[] GetKnownLocationsforPatientID(string currentPatientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            Location[] knownLocations = db.Locations.Where(p => p.PatientID == currentPatientID).ToArray();

            return knownLocations;
        }

        // PATCH tables/Location/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Location> PatchLocation(string id, Delta<Location> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Location
        public async Task<IHttpActionResult> PostLocation(Location item)
        {
            Location current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Location/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteLocation(string id)
        {
             return DeleteAsync(id);
        }
    }
}
