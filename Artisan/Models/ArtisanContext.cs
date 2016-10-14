namespace Artisan.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ArtisanContext : DbContext
    {
        public ArtisanContext()
            : base("SomeeConnection")
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Image> Images { get; set; }


    }
}
