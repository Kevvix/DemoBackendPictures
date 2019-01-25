using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Demo.Backend.Pictures.Models;
using System.Collections.Generic;

namespace Demo.Backend.Pictures.Controllers
{

    /// <summary>
    /// Version du contrôleur utilisant un système de stockage sur le réseau
    /// </summary>
    public class PhotosWithNetworkStorageStrategyController : ApiController
    {
        [HttpPost]
        [ValidateFiles]
        public HttpResponseMessage Create()
        {
            var photos = new List<Photo>();
            using (var entities = new Entities())
            {
                var files = HttpContext.Current.Request.Files.Collection();
                foreach (var file in files)
                {
                    var storageKey = Guid.NewGuid();
                    var photo = new PhotoWithNetworkStorageStrategy {
                        MimeType = file.ContentType,
                        FilenameWithExtension = file.FileName,
                        StorageKey = storageKey
                    };
                    photos.Add(photo);
                    var path = PathToStorageKey(storageKey.ToString());
                    file.SaveAs(path);
                }
                entities.Photos.AddRange(photos);
                entities.SaveChanges();

            }
            return Request.CreateResponse(HttpStatusCode.OK, photos);
        }
            
        [HttpGet]
        public HttpResponseMessage GetPhoto(int id)
        {
            using (var entities = new Entities())
            {
                var photo = entities.Photos.Find(id);
                if (photo == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Image non-trouvée");
                }
                var photoWithNetworkStorageStrategy = (PhotoWithNetworkStorageStrategy)photo;
                var path = PathToStorageKey(photoWithNetworkStorageStrategy.StorageKey.ToString());
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(path, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = photo.FilenameWithExtension;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(photo.MimeType);
                result.Content.Headers.ContentLength = stream.Length;
                return result;
            }
        }

        private string PathToStorageKey(string storageKey)
        {
            return Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Images"), storageKey);
        }

    }
}
