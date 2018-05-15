using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashMe.Admin.Models
{
    public class Pending
    {
        public string account { get; set; }
        public int pending { get; set; }
    }

    public class RootObject
    {
        public string maxdistributed { get; set; }
        public string threshold { get; set; }
        public double claimrate { get; set; }
        public int claimingnow { get; set; }
        public int eta { get; set; }
        public double reward { get; set; }
        public string top_claims { get; set; }
        public string top_accounts { get; set; }
        public string nontop_claims { get; set; }
        public string nontop_accounts { get; set; }
        public List<Pending> pending { get; set; }
    }
    public class ResultWallet
    {
        public int Coin { get; set; }
        public double RealValue { get; set; }
    }
    public class PriceRaiblock
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string rank { get; set; }
        public string price_usd { get; set; }
        public string price_btc { get; set; }
        public string __invalid_name__24h_volume_usd { get; set; }
        public string market_cap_usd { get; set; }
        public string available_supply { get; set; }
        public string total_supply { get; set; }
        public string percent_change_1h { get; set; }
        public string percent_change_24h { get; set; }
        public string percent_change_7d { get; set; }
        public string last_updated { get; set; }
    }
}