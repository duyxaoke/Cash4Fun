using System;
using System.Collections.Generic;

namespace CashMe.Service.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserId { get; set; }
        public string ToUserName { get; set; }
        public List<string> ListRoles { get; set; }
        public List<string> ListUsers { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public System.DateTime? CreateDate { get; set; }

    }
}