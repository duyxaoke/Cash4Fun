using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using System.Linq;
using System;
using CashMe.Service.CacheService;

namespace CashMe.Service
{
    public interface IWalletServices
    {
        IEnumerable<Wallet> GetAllWallet();
        Wallet GetWallet(int id);
        Wallet GetWalletbyCode(string code);
        IEnumerable<Wallet> GetWalletInActiveStore();
        void InserWallet(Wallet model);
        void UpdateWallet(Wallet model);
        void UpdateWalletStore(int walletid, int claim, int asktillnow);
        IEnumerable<Wallet> getWalletStore(string userid);
        MainViewModel getMainView();
        void Save();
        void Dispose();

    }
    public class WalletServices : IWalletServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        readonly ICacheProviderService _cacheProviderService = new CacheProviderService();
        public IEnumerable<Wallet> _getDatatrackingWallet()
        {
            // First, check the cache
            var trackingdata = _cacheProviderService.Get("TrackingWallet") as IDictionary<int, Wallet>;

            // If it's not in the cache, we need to read it from the repository
            if (trackingdata == null)
            {
                //kiem tra thong bao trong ngay
                trackingdata = _unitOfWork.WalletRepository.GetAll().ToDictionary(v => v.Id);

                if (trackingdata.Any())
                {
                    // Put this data into the cache for 300 minutes
                    _cacheProviderService.Set("TrackingWallet", trackingdata); // 300 minute
                }
            }
            return trackingdata.Values;
        }

        public IEnumerable<Wallet> GetAllWallet()
        {
            return _unitOfWork.WalletRepository.ExecWithStoreProcedure("SELECT * FROM Wallets WITH(NOLOCK) ORDER BY Claim DESC");
        }
        public Wallet GetWallet(int id)
        {
            return _unitOfWork.WalletRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM Wallets WITH(NOLOCK) WHERE Id = {0}", id)).FirstOrDefault();
        }
        public Wallet GetWalletbyCode(string code)
        {
            return _unitOfWork.WalletRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM Wallets WITH(NOLOCK) WHERE Code = '{0}'", code)).FirstOrDefault();
        }
        public IEnumerable<Wallet> GetWalletInActiveStore()
        {
            return _unitOfWork.WalletRepository.ExecWithStoreProcedure("SELECT * FROM Wallets WITH(NOLOCK) WHERE Status = 1 AND datediff(minute, UpdateDate, GETDATE()) > 15 ORDER BY Claim DESC");
        }
        public void InserWallet(Wallet model)
        {
            _unitOfWork.WalletRepository.Insert(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingWallet");
            #endregion

        }
        public void UpdateWallet(Wallet model)
        {
            _unitOfWork.WalletRepository.Update(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingWallet");
            #endregion

        }
        public IEnumerable<Wallet> getWalletStore(string userid)
        {
            return _unitOfWork.WalletRepository.ExecWithStoreProcedure(String.Format("EXEC GetWallet '{0}'", userid));

        }
        public MainViewModel getMainView()
        {
            return _unitOfWork.MainViewRepository.ExecWithStoreProcedure("EXEC MainView").FirstOrDefault();

        }

        public void UpdateWalletStore(int walletid, int claim, int asktillnow)
        {
            _unitOfWork.WalletRepository.ExecStore(String.Format("EXEC Update_Coin_Wallets {0},{1},{2}", walletid, claim, asktillnow));

        }
        public void Save()
        {
            _unitOfWork.Save();
        }
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
