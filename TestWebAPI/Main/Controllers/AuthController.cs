using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;

using Main.Common;
using Main.Models;

namespace Main.Controllers {

    public class AuthController : ApiController {
        private AddDbContext db = new AddDbContext();

        // POST: api/Auth
        [HttpPost]
        [ResponseType(typeof(LoginResult))]
        public async Task<IHttpActionResult> PostAuth(LoginParameter param) {
            Member r = await db.M_Members
                .FindAsync(param.UserID);

            if (r == null) { return NotFound(); }
            if (r.LoginPassword != param.Password) { return NotFound(); }

            FormsAuthentication.SetAuthCookie(param.UserID, false);

            LoginResult result = new LoginResult { UserName = r.MemberName, AuthorityID = r.AuthorityID };
            return Ok(result);
        }
    }

    public class LoginParameter {
        public string UserID { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult {
        public string UserName { get; set; }
        public string AuthorityID { get; set; }
    }
}
