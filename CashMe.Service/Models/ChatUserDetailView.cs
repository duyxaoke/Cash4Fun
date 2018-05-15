using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMe.Service.Models
{
    public class ChatUserDetailView
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ImageRank { get; set; }
        public string TextColor { get; set; }
    }
}
