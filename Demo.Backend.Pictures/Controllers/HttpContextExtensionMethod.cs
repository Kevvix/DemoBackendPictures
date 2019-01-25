using System.Web;
using System.Collections.Generic;

namespace Demo.Backend.Pictures.Controllers
{
    public static class HttpContextExtensionMethod {

        /// <summary>
        /// Ajouter une méthode d'extension à HttpFileCollection pour être en mesure d'avoir une liste énumérable des fichiers
        /// </summary>
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
