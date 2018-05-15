using System;
namespace CashMe.Core.Data
{
    public class Config
    {
        public int Id { get; set; }
        public int HoaHong { get; set; }
        public float GiaCoin { get; set; }
        public int ImageForNewMember { get; set; }
        public int MailSendToday { get; set; }
        public string Version { get; set; }
        public bool IsOnline { get; set; }
    }
}
