﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class BankAccountInfo
    {
        [Key]
        public int BankAccountInfoId { get; set; }

        [MaxLength(50)]
        public string BankName { get; set; }

        [MaxLength(20)]
        public string AccountNumberMasked { get; set; }

        [MaxLength(20)]
        public string RoutingNumber { get; set; }

        [MaxLength(20)]
        public string AccountType { get; set; } // e.g., "Checking", "Savings"

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation property
        public ICollection<PreferredMethod> PreferredMethods { get; set; }
    }
}