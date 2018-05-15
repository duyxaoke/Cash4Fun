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
    public interface ITargetMasterServices
    {
        IEnumerable<TargetMaster> GetAllTargetMaster();
        TargetMaster GetTargetMaster(int id);
        void InserTargetMaster(TargetMaster model);
        void UpdateTargetMaster(TargetMaster model);

        void Save();
        void Dispose();

    }
    public class TargetMasterServices : ITargetMasterServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        readonly ICacheProviderService _cacheProviderService = new CacheProviderService();
        public IEnumerable<TargetMaster> _getDatatrackingTargetMaster()
        {
            // First, check the cache
            var trackingdata = _cacheProviderService.Get("TrackingTargetMaster") as IDictionary<int, TargetMaster>;

            // If it's not in the cache, we need to read it from the repository
            if (trackingdata == null)
            {
                //kiem tra thong bao trong ngay
                trackingdata = _unitOfWork.TargetMasterRepository.GetAll().ToDictionary(v => v.Id);

                if (trackingdata.Any())
                {
                    // Put this data into the cache for 300 minutes
                    _cacheProviderService.Set("TrackingTargetMaster", trackingdata); // 300 minute
                }
            }
            return trackingdata.Values;
        }

        public IEnumerable<TargetMaster> GetAllTargetMaster()
        {
            return _getDatatrackingTargetMaster();
        }
        public TargetMaster GetTargetMaster(int id)
        {
            return _getDatatrackingTargetMaster().FirstOrDefault(c=>c.Id == id);
        }
        public void InserTargetMaster(TargetMaster model)
        {
            _unitOfWork.TargetMasterRepository.Insert(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingTargetMaster");
            #endregion

        }
        public void UpdateTargetMaster(TargetMaster model)
        {
            _unitOfWork.TargetMasterRepository.Update(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingTargetMaster");
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
