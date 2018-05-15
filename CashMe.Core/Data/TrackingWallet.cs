using System;
namespace CashMe.Core.Data
{
    public class TrackingWallet
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int ClaimInHour { get; set; }
        public int LastActive { get; set; }
    }
}
