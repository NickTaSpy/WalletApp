using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletApp.Infrastructure.Database.Tables;

public class CurrencyRate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column(TypeName = "Date")]
    [Required]
    public DateTime Date { get; set; }

    [MaxLength(3)]
    [Required]
    public required string Currency { get; set; }

    [Precision(10, 5)]
    [Required]
    public decimal Rate { get; set; }
}
