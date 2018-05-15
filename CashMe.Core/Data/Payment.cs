using System;
namespace CashMe.Core.Data
{
    public class Payment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double Amount { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
        public string ComputerName { get; set; }
        public int TypePayment { get; set; }

        public System.DateTime CreateDate { get; set; }
    }
}
