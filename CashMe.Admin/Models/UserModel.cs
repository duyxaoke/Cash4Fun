using System;
using System.ComponentModel.DataAnnotations;

namespace CashMe.Admin.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}