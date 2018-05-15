using System;
using System.Collections.Generic;

namespace CashMe.Service.Models
{
    public class UserRefModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Nullable<int> CountImage { get; set; }

    }
}