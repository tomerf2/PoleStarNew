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
    public class GroupController : TableController<Group>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Group>(context, Request);
        }

        // GET tables/Group
        public IQueryable<Group> GetAllGroup()
        {
            return Query(); 
        }

        // GET tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Group> GetGroup(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Group> PatchGroup(string id, Delta<Group> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Group
        public async Task<IHttpActionResult> PostGroup(Group item)
        {
            Group current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGroup(string id)
        {
             return DeleteAsync(id);
        }
    }
}
