namespace Artisan.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Mail
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Минимум 3, максимум 20")]
        [Display(Name = "Ваше имя*:")]
        public string Name { get; set; }
        
        public string Producer { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Неверный формат электронной почты")]
        [Display(Name = "Ваш Email*:")]
        public string EmailTo { get; set; }

        [Required(ErrorMessage = "Заполните пожалуйста поле")]
        [Display(Name = "Ваш телефон*:")]
        public string Telephone { get; set; }

        [Display(Name = "Сообщение:")]
        public string Message { get; set; }
        
        public DateTime Date { get; set; }
    }
}