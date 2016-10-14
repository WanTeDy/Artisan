namespace Artisan.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;


    public class Image
    {       
        public int Id { get; set; }
        
        [Required]        
        public string PublicId { get; set; }

        public string Description { get; set; }

        public bool Gallery { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<Product> Products { get; set; }

        public Image()
        {
            Categories = new List<Category>();
            Products = new List<Product>();
        }
    }
}