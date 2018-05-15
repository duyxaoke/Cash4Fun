using System;
namespace CashMe.Core.Data
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int WalletId { get; set; }
        public float Amount { get; set; }
        public string IP { get; set; }
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public string ImageRank { get; set; }
        public string TextColor { get; set; }
        public string MAC { get; set; }
        public string ComputerName { get; set; }
        public int FlagLogin { get; set; }//App, Web
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Password { get; set; }

    }
}
