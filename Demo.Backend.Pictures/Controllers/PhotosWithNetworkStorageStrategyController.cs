using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Demo.Backend.Pictures.Models;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.Primitives;
using WebApi.OutputCache.V2;

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
        [Route("api/PhotosWithNetworkStorageStrategy/getPhoto/{id}")]
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
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = photo.FilenameWithExtension;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(photo.MimeType);
                result.Content.Headers.ContentLength = stream.Length;
                return result;
            }
        }
        
        [HttpGet]
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [Route("api/PhotosWithNetworkStorageStrategy/GetPhotoUsingResizer/{id}/{maxWidthOrHeigth}")]
        public HttpResponseMessage GetPhotoUsingResizer(int id, int maxWidthOrHeigth)
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
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var ms = new MemoryStream();
                var image = Image.Load(stream);
                image.Mutate(x => x.Resize(CalculateNewSize(image, maxWidthOrHeigth)));
                image.Save(ms, new JpegEncoder() { Quality = 80 });
                ms.Seek(0, SeekOrigin.Begin);
                result.Content = new StreamContent(ms);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = photo.FilenameWithExtension;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(photo.MimeType);
                return result;
            }
        }

        private Size CalculateNewSize(Image<Rgba32> image, int maxWidthOrHeigth)
        {
            int width = image.Width;
            int heigth = image.Height;
            var moreLong = image.Width > image.Height ? width : heigth;
            if (moreLong > maxWidthOrHeigth)
            {
                var ratio = (double)maxWidthOrHeigth / (double)moreLong;
                width = (int)(width * ratio);
                heigth = (int)(heigth * ratio);
            }
            var ResizeOption = new ResizeOptions();
            return new Size(width, heigth);
        }
        
        private string PathToStorageKey(string storageKey)
        {
            return Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Images"), storageKey);
        }

    }
}
