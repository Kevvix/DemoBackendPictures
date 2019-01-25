using Demo.Backend.Pictures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Demo.Backend.Pictures.Controllers
{
    public class PhotosController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage GetPhotos()
        {
            using (var entities = new Entities())
            {
                var data = entities.Photos.ToList().Select(p => new
                {
                    p.Id,
                    p.MimeType,
                    p.FilenameWithExtension,
                    type = p.GetType().Name
                });
                return Request.CreateResponse(data);
            }
        }
    }
}