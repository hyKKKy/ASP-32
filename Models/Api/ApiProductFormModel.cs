using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_32.Models.Api
{
    public class ApiProductFormModel
    {

        [FromForm(Name = "product-group-id")]
        public String GroupId { get; set; } = null!;

        [Required(ErrorMessage = "Поле є обов'язкове.")]
        [FromForm(Name = "product-name")]
        public String Name { get; set; } = null!;

        [Required(ErrorMessage = "Поле є обов'язкове.")]
        [FromForm(Name = "product-description")]
        public String? Description { get; set; } = null!;

        [Required(ErrorMessage = "Поле є обов'язкове.")]
        [FromForm(Name = "product-slug")]
        public String? Slug { get; set; } = null!;

        [Required(ErrorMessage = "Поле є обов'язкове.")]
        [FromForm(Name = "product-img")]
        public IFormFile? Image { get; set; } = null!;

        [Required(ErrorMessage = "Поле є обов'язкове.")]
        [FromForm(Name = "product-price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Поле є обов'язкове.")]
        [FromForm(Name = "product-stock")]
        public int Stock { get; set; }
    }
}
