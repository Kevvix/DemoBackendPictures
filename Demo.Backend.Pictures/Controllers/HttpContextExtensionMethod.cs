using System.Web;
using System.Collections.Generic;

namespace Demo.Backend.Pictures.Controllers
{
    public static class HttpContextExtensionMethod {

        public static IEnumerable<HttpPostedFile> Collection(this HttpFileCollection files) {
            var data = new List<HttpPostedFile>();
            for (var i = 0; i < files.Count; i++)
            {
                data.Add(files[i]);
            }
            return data;
        }

    }
}
