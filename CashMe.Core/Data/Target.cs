using System;
namespace CashMe.Core.Data
{
    public class Target
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CountImage { get; set; }
        public int StatusBonus { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
