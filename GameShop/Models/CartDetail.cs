using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShop.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCart_Id {  get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public Game Game { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
