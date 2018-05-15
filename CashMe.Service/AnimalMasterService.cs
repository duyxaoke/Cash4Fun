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
    public interface IAnimalMasterServices
    {
        IEnumerable<AnimalMaster> GetAllAnimalMaster();
        AnimalMaster GetAnimalMaster(int id);
        AnimalMaster GetAnimalMasterbyKey(string key);
        void InserAnimalMaster(AnimalMaster model);
        void UpdateAnimalMaster(AnimalMaster model);

        void Save();
        void Dispose();

    }
    public class AnimalMasterServices : IAnimalMasterServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        readonly ICacheProviderService _cacheProviderService = new CacheProviderService();
        public IEnumerable<AnimalMaster> _getDatatrackingAnimalMaster()
        {
            // First, check the cache
            var trackingdata = _cacheProviderService.Get("TrackingAnimalMaster") as IDictionary<int, AnimalMaster>;

            // If it's not in the cache, we need to read it from the repository
            if (trackingdata == null)
            {
                //kiem tra thong bao trong ngay
                trackingdata = _unitOfWork.AnimalMasterRepository.GetAll().ToDictionary(v => v.Id);

                if (trackingdata.Any())
                {
                    // Put this data into the cache for 300 minutes
                    _cacheProviderService.Set("TrackingAnimalMaster", trackingdata); // 300 minute
                }
            }
            return trackingdata.Values;
        }

        public IEnumerable<AnimalMaster> GetAllAnimalMaster()
        {
            return _getDatatrackingAnimalMaster();
        }
        public AnimalMaster GetAnimalMaster(int id)
        {
            return _getDatatrackingAnimalMaster().FirstOrDefault(c=>c.Id == id);
        }
        public AnimalMaster GetAnimalMasterbyKey(string key)
        {
            return _getDatatrackingAnimalMaster().FirstOrDefault(c=>c.Key == key);
        }
        public void InserAnimalMaster(AnimalMaster model)
        {
            _unitOfWork.AnimalMasterRepository.Insert(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingAnimalMaster");
            #endregion

        }
        public void UpdateAnimalMaster(AnimalMaster model)
        {
            _unitOfWork.AnimalMasterRepository.Update(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingAnimalMaster");
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
