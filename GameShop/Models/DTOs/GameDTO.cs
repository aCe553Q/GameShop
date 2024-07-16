using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GameShop.Models.DTOs
{
    public class GameDTO
{
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? GameName { get; set; }

        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; }
        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }
        [Required]
        public int GenreId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }






    }
 
}
