using System;
namespace CashMe.Core.Data
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int Claim { get; set; }
        public int AskStillNow { get; set; }
        public int Status { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
