using System.ComponentModel.DataAnnotations;

namespace GameShop.Models
{
    public class CheckoutModel
    {
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Required]
        public string? Number { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Address { get; set; }

        [Required]
        public string? PaymentMethod { get; set; }
    }
}
