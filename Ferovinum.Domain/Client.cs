using System.ComponentModel.DataAnnotations;

namespace Ferovinum.Domain
{
    public class Client
    {
        [Key]
        [MaxLength(10)]
        public required string Id { get; set; }

        public required float Fee { get; set; }
    }
}
