using System.ComponentModel.DataAnnotations;

namespace Ferovinum.Domain
{
    public class Product
    {
        [Key]
        [MaxLength(10)]
        public required string Id { get; set; }

        public required float Price { get; set; }
    }
}
