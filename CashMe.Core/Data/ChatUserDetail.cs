﻿using System;
namespace CashMe.Core.Data
{
    public class ChatUserDetail
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
