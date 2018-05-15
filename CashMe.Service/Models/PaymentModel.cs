using System;
using System.Collections.Generic;

namespace CashMe.Service.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Nullable<int> CountImage { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Content { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }

    }
}