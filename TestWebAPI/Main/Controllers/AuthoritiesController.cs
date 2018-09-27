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
    public class AuthoritiesController : ApiController {
        private AddDbContext db = new AddDbContext();

        // GET(Select All): api/Authorities
        [HttpGet]
        [ResponseType(typeof(IQueryable<Authority>))]
        public IQueryable<Authority> GetAuthorities() {
            return db.M_Authorities;
        }

        // GET(Select): api/Authorities/id
        [HttpGet]
        [ResponseType(typeof(Authority))]
        public async Task<IHttpActionResult> GetAuthority(string id) {
            Authority r = await db.M_Authorities.FindAsync(id);

            if (r == null) { return NotFound(); }

            return Ok(r);
        }

        // POST(INSERT): api/Authorities
        [HttpPost]
        [ResponseType(typeof(Authority))]
        public async Task<IHttpActionResult> PostAuthority(Authority r) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            db.M_Authorities.Add(r);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = r.AuthorityID }, r);
        }

        // PUT(Update): api/Authorities/id
        [HttpPut]
        [ResponseType(typeof(Authority))]
        public async Task<IHttpActionResult> PutAuthority(string id, Authority r) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != r.AuthorityID) return BadRequest();

            db.Entry(r).State = EntityState.Modified;

            try {
                await db.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!AuthorityExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = r.AuthorityID }, r);
        }

        // DELETE(Delete): api/Authorities/id
        [HttpDelete]
        [ResponseType(typeof(Authority))]
        public async Task<IHttpActionResult> DeleteAuthority(string id) {
            Authority r = await db.M_Authorities.FindAsync(id);

            if (r == null) return NotFound();

            db.M_Authorities.Remove(r);
            await db.SaveChangesAsync();

            return Ok(r);
        }

        private bool AuthorityExists(string id) {
            return db.M_Authorities.Count(e => e.AuthorityID == id) > 0;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
