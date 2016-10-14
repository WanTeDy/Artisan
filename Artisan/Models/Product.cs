namespace Artisan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Поле не должно быть меньше 3-х и больше 50-ти символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        [Range(0, Double.MaxValue, ErrorMessage = "Цена не может быть отрицательна")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }
        
        public int ImageId { get; set; }

        public Image Image { get; set; }

        public Category Category { get; set; }
    }
}
