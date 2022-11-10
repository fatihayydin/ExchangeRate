using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeRate.Data.Data
{
    //[Index(nameof(ApiKey),nameof(CreatedDate), nameof(HttpStatusCode), nameof(Direction))]
    public class CustomerApiLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? ApiKey { get; set; }
        [Required]
        public Direction Direction { get; set; }
        [Required]
        public int HttpStatusCode { get; set; }
        [Required]
        public string? Message { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }

    public enum Direction
    {
        Incoming = 0,
        Outgoing = 1
    }
}
