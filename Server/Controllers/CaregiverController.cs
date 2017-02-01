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
    public class CaregiverController : TableController<Caregiver>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Caregiver>(context, Request, enableSoftDelete: true);
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
