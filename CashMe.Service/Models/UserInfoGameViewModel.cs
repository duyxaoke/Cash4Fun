using System;
using System.Collections.Generic;

namespace CashMe.Service.Models
{
    public class UserInfoGameViewModel
    {
        public string UserId { get; set; }
        public int TotalCoin { get; set; }
        public int RealCoin { get; set; }
        public float Amount { get; set; }
        public int TotalBet { get; set; }

    }
}