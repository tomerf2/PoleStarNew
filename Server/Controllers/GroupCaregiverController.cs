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
    public class GroupCaregiverController : TableController<GroupCaregiver>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<GroupCaregiver>(context, Request, enableSoftDelete: true);
        }

        // GET tables/GroupCaregiver
        public IQueryable<GroupCaregiver> GetAllGroupCaregiver()
        {
            return Query(); 
        }

        // GET tables/GroupCaregiver/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<GroupCaregiver> GetGroupCaregiver(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/GroupCaregiver/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<GroupCaregiver> PatchGroupCaregiver(string id, Delta<GroupCaregiver> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/GroupCaregiver
        public async Task<IHttpActionResult> PostGroupCaregiver(GroupCaregiver item)
        {
            GroupCaregiver current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/GroupCaregiver/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGroupCaregiver(string id)
        {
             return DeleteAsync(id);
        }
    }
}
