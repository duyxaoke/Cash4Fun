using System;
namespace CashMe.Core.Data
{
    public class Game
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public int Image { get; set; }
        public float Result { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
