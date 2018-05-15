using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using Quartz;
using System.Transactions;
using CashMe.Service;
using CashMe.Admin.Models;
using HtmlAgilityPack;

namespace CashMe.Admin.Scheduler
{
    public class AutoUpdateTarget : IJob
    {
        private WalletServices _walletServices = new WalletServices();
        private AccountServices _AccountServices = new AccountServices();
        private UserInfoServices _userInfoServices = new UserInfoServices();
        private ClaimsServices _claimsServices = new ClaimsServices();
        private TargetServices _targetServices = new TargetServices();
        private TargetMasterServices _targetMasterServices = new TargetMasterServices();

        public void Execute(IJobExecutionContext context)
        {
            Set();
        }
        public void Set()
        {
            try
            {
                //goi store Reset_Target
                _targetServices.ResetTarget();
            }
            catch (Exception)
            {
            }
        }
    }
}