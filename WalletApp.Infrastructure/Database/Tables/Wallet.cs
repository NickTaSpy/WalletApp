﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletApp.Infrastructure.Database.Tables;

public class Wallet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Precision(19, 4)]
    [Required]
    public decimal Balance { get; set; }

    [Unicode(false)]
    [MaxLength(3)]
    [Required]
    public string Currency { get; set; } = "EUR";
}