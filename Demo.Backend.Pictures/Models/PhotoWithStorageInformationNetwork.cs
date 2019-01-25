using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Backend.Pictures.Models
{
    
    public class Photo
    {
        public int Id { get; set; }
        [Required]
        public string FilenameWithExtension { get; set; }
        [Required]
        public string MimeType { get; set; }
    }

    public class PhotoWithNetworkStorageStrategy : Photo
    {
        public Guid StorageKey { get; set; }
    }

    public class PhotoWithDatabaseStorageStrategy : Photo
    {
        public byte[] Content { get; set; }
    }

}