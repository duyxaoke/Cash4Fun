using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using CashMe.Service.CacheService;
using System.Linq;

namespace CashMe.Service
{
    public interface IConfigServices
    {
        IEnumerable<Config> GetAllConfig();
        Config GetConfig();
        void UpdateConfig(Config model);

        void Save();
        void Dispose();

    }
    public class ConfigServices : IConfigServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        readonly ICacheProviderService _cacheProviderService = new CacheProviderService();
        public IEnumerable<Config> _getDatatrackingConfig()
        {
            // First, check the cache
            var trackingdata = _cacheProviderService.Get("TrackingConfig") as IDictionary<int, Config>;

            // If it's not in the cache, we need to read it from the repository
            if (trackingdata == null)
            {
                //kiem tra thong bao trong ngay
                trackingdata = _unitOfWork.ConfigRepository.GetAll().ToDictionary(v => v.Id);

                if (trackingdata.Any())
                {
                    // Put this data into the cache for 300 minutes
                    _cacheProviderService.Set("TrackingConfig", trackingdata); // 300 minute
                }
            }
            return trackingdata.Values;
        }

        public IEnumerable<Config> GetAllConfig()
        {
            return _getDatatrackingConfig();
        }
        public Config GetConfig()
        {
            return _unitOfWork.ConfigRepository.GetAll().FirstOrDefault(c=>c.Id == 1);
        }
        public void UpdateConfig(Config model)
        {
            _unitOfWork.ConfigRepository.Update(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingConfig");
            #endregion

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
