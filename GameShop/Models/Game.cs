using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShop.Models
{
    [Table("Game")]
    public class Game
    {
        
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string? GameName { get; set; }
        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre Genre {  get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List <CartDetail> CartDetail { get; set; }
    }
}
