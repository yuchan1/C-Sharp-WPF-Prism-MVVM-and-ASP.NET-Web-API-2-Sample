using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using Main.Common;
using Main.Models;

namespace Main.Controllers {

    [Authorize]
    public class MembersController : ApiController {
        private AddDbContext db = new AddDbContext();

        // GET(Select All): api/Members
        [HttpGet]
        [ResponseType(typeof(IQueryable<Member>))]
        public IQueryable<Member> GetMembers() {
            return db.M_Members;
        }

        // GET(Select): api/Members/id
        [HttpGet]
        [ResponseType(typeof(Member))]
        public async Task<IHttpActionResult> GetMember(string id) {
            Member r = await db.M_Members.FindAsync(id);

            if (r == null) { return NotFound(); }

            return Ok(r);
        }

        // POST(INSERT): api/Members
        [HttpPost]
        [ResponseType(typeof(Member))]
        public async Task<IHttpActionResult> PostMember(Member r) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            db.M_Members.Add(r);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = r.MemberID }, r);
        }

        // PUT(Update): api/Members/id
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMember(string id, Member r) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != r.MemberID) return BadRequest();

            db.Entry(r).State = EntityState.Modified;

            try {
                await db.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!MemberExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }
            
            return CreatedAtRoute("DefaultApi", new { id = r.MemberID }, r);
        }

        // DELETE(Delete): api/Members/id
        [HttpDelete]
        [ResponseType(typeof(Member))]
        public async Task<IHttpActionResult> DeleteMember(string id) {
            Member r = await db.M_Members.FindAsync(id);

            if (r == null) return NotFound();

            db.M_Members.Remove(r);
            await db.SaveChangesAsync();

            return Ok(r);
        }

        private bool MemberExists(string id) {
            return db.M_Members.Count(e => e.MemberID == id) > 0;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
