using System;
using System.Collections.Generic;

namespace CashMe.Service.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Target { get; set; }
        public int CountImage { get; set; }
        public int StatusBonus { get; set; }
        public int Ref { get; set; }
        public int WalletId { get; set; }
        public float Amount { get; set; }
        public string ImageRank { get; set; }
        public string TextColor { get; set; }

    }
}