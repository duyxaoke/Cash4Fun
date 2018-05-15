using System;
namespace CashMe.Core.Data
{
    public class Bet
    {
        public Guid Id { get; set; }
        public string ResultRaceId { get; set; }
        public string UserId { get; set; }
        public int Animal1 { get; set; }
        public int Coin1 { get; set; }
        public int Animal2 { get; set; }
        public int Coin2 { get; set; }
        public int Change { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
