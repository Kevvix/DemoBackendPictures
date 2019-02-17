using Demo.Backend.Pictures.Models;
using System.Linq;
using System.Web.Mvc;

namespace Demo.Backend.Pictures.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (var entities = new Entities())
            {
                var photo = entities.Photos.ToList().LastOrDefault();
                if(photo != null)
                {
                    var url = photo is PhotoWithDatabaseStorageStrategy ? "PhotosWithDatabaseStorageStrategy" : "PhotosWithNetworkStorageStrategy";
                    ViewBag.url = url;
                }
                ViewBag.model = photo;
                return View();
            }
        }
    }
}