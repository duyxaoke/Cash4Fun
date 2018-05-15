using System;
namespace CashMe.Core.Data
{
    public class TargetMaster
    {
        public int Id { get; set; }
        public int Higher { get; set; } // lam hon bao nhiu? > 100 claim chang han
        public int Bonus { get; set; } //thuong
        public string Color { get; set; } //mau
        public DateTime CreateDate { get; set; }
    }
}
