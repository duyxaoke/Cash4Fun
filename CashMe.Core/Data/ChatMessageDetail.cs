using System;
namespace CashMe.Core.Data
{
    public class ChatMessageDetail
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
