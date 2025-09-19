using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.API.Models
{
    public class CreateTransactionRequest
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty; // "income" or "expense"

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }
    }
}