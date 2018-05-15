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
    public class AutoUpdate : IJob
    {
        private WalletServices _walletServices = new WalletServices();
        private AccountServices _AccountServices = new AccountServices();
        private UserInfoServices _userInfoServices = new UserInfoServices();
        private ClaimsServices _claimsServices = new ClaimsServices();
        private TargetServices _targetServices = new TargetServices();

        public void Execute(IJobExecutionContext context)
        {
            Set();
        }

        public void Set()
        {
            var wallet = _walletServices.GetWalletInActiveStore().ToArray();
            for (int item = 0; item < wallet.Count(); item++)
            {
                try
                {
                    var userInfo = _userInfoServices.GetUserInfobyWallet(wallet[item].Id);

                    string UserId = userInfo == null ? string.Empty : userInfo.UserId;

                    TimeSpan minuteWallet = DateTime.Now - wallet[item].UpdateDate;
                    if (minuteWallet.TotalMinutes >= 15)
                    {
                        _claimsServices.UpdateClaims_30Minute(UserId, wallet[item].Id, 0, 0, 0);
                        //using (WebClient webClient = new WebClient())
                        //{
                        //    string baseAddress = String.Format("https://faucet.raiblockscommunity.net/paylist.php?acc={0}&json=1", wallet[item].Code);
                        //    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
                        //    webClient.Headers.Add(HttpRequestHeader.Accept, "application/json, text/javascript, */*; q=0.01");
                        //    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
                        //    webClient.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        //    string baseSiteString = webClient.DownloadString(baseAddress);
                        //    if (baseSiteString.Contains("ask-till-now") && baseSiteString.Contains(wallet[item].Code))
                        //    {
                        //        string sourceClaim = Regex.Match(baseSiteString, wallet[item].Code + "(.*?),\"ask-till-now").Value;
                        //        string sourceAskTillNow = Regex.Match(baseSiteString, "ask-till-now\":(.*?),\"expected-pay").Value;
                        //        int claim = int.Parse(sourceClaim.Replace(wallet[item].Code + "\",\"pending\":", "").Replace(",\"ask-till-now", ""));
                        //        int asktillnow = int.Parse(sourceAskTillNow.Replace("ask-till-now\":", "").Replace(",\"expected-pay", ""));
                        //        //thuc thi store: 1= co du lieu
                        //        _claimsServices.UpdateClaims_30Minute(UserId, wallet[item].Id, asktillnow, claim, 1);

                        //    }
                        //    else if (baseSiteString.Contains("\"pending\":[]"))
                        //    {
                        //        //thuc thi store: 0= ko co du lieu
                        //        _claimsServices.UpdateClaims_30Minute(UserId, wallet[item].Id, 0, 0, 0);
                        //    }

                        //    webClient.Dispose();
                        //}
                    }
                }

                catch (Exception ex)
                {
                    continue;
                }

            }

            //update wallet UserInfo, if Wallet Status = 0
            _userInfoServices.UpdateWalletUserInfoByStore();

        }
    }
}