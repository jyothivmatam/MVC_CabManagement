using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MVC_CabServices.Controllers
{
    public class BaseController : Controller
    {
        private const string Sessionkey = "token";
        public const string id = "id";
        public JsonSerializerSettings jsonSettings;
        public const string SessionAdminOrUser = "checkAdminOrUser";
        public BaseController()
        {
            jsonSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Error
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string? loginID(string sessionkey)
        {
          var test = HttpContext.Session.GetString(sessionkey);
            return test;
        }
        public int? getid(string sessionid)
        {
            var test = HttpContext.Session.GetInt32(sessionid);
            return test;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public int? getadmin(string sessionid)
        {
            var test = HttpContext.Session.GetInt32(sessionid);
            return test;
        }
    }
}
