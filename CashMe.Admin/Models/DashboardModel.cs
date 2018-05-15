using CashMe.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashMe.Admin.Models
{
    public class DashboardModel
    {
        public double RealValue { get; set; }
        public double CountImage { get; set; }
        public int TotalRef { get; set; }
        public float GiaCoin { get; set; }
        public string UserId { get; set; }
        public IEnumerable<TargetMaster> lsTargetMaster { get; set; }
        public Target Target { get; set; }

    }
}