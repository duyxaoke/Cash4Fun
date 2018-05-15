using System;
namespace CashMe.Core.Data
{
    public class ChatPrivateMessageDetail
    {
        public Guid Id { get; set; }
        public string MasterUserId { get; set; }
        public string ChatToUserId { get; set; }
        public string Message { get; set; }
    }
}
