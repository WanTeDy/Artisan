namespace Artisan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Category
    {        
        public int Id { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        [MinLength(3, ErrorMessage = "Поле не должно быть меньше 3-х символов")]
        [MaxLength(50, ErrorMessage = "Поле не должно быть больше 50-ти символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        public string Description { get; set; }
        
        public int ImageId { get; set; }
        
        public Image Image { get; set; }

        public ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new List<Product>();
        }
    }
}
