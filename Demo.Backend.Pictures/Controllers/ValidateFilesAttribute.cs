using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace Demo.Backend.Pictures.Controllers
{
    public class ValidateFilesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var currentRequest = HttpContext.Current.Request;
            if (currentRequest.Files.Count == 0)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "Vous n'avez pas spécifié de fichier");
                return;
            }
            
            var files = HttpContext.Current.Request.Files.Collection();
            foreach (var file in files)
            { 
                if (file == null && file.ContentLength == 0)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, currentRequest.Files.Count == 1 ? "Le fichier spécifié est vide" : "L'un des fichiers spécifié est vide");
                    return;
                }
            }
            base.OnActionExecuting(actionContext);
        }
    }
}
