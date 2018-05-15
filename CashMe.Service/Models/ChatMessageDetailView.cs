using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMe.Service.Models
{
    public class ChatMessageDetailView
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string ImageRank { get; set; }
        public string TextColor { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
