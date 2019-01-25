namespace Demo.Backend.Pictures.Models
{
    using System.Data.Entity;

    public class Entities : DbContext
    {
        public Entities() : base("name=Entities") { }
        public virtual DbSet<Photo> Photos { get; set; }
    }
}