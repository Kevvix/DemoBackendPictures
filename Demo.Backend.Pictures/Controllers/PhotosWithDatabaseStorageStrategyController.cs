﻿using System.Linq;
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
    /// Version du contrôleur utilisant un système de stockage dans la BD
    /// </summary>
    public class PhotosWithDatabaseStorageStrategyController : ApiController
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
                    using (var memoryStream = new MemoryStream())
                    {
                        file.InputStream.CopyTo(memoryStream);
                        var bytes = memoryStream.ToArray();
                        var photo = new PhotoWithDatabaseStorageStrategy
                        {
                            MimeType = file.ContentType,
                            FilenameWithExtension = file.FileName,
                            Content = bytes
                        };
                        photos.Add(photo);
                    }
                }
                entities.Photos.AddRange(photos);
                entities.SaveChanges();

            }
            return Request.CreateResponse(HttpStatusCode.OK, photos.Select(p => new {
               p.Id,
               p.MimeType,
               p.FilenameWithExtension
            }));
        }
            
        [HttpGet]
        public HttpResponseMessage GetPhoto(int id)
        {
            using (var entities = new Entities())
            {
                var photo = entities.Photos.Find(id);
                if(photo == null) {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Image non-trouvée");
                }
                var photoWithDatabaseStorageStrategy = (PhotoWithDatabaseStorageStrategy)photo;
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(photoWithDatabaseStorageStrategy.Content);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = photo.FilenameWithExtension;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(photo.MimeType);
                result.Content.Headers.ContentLength = photoWithDatabaseStorageStrategy.Content.Length;
                return result;
            }
        }

    }
}
