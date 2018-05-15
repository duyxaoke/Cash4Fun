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
    public interface IPaymentServices
    {
        IEnumerable<Payment> GetAllPayment();
        IEnumerable<Payment> GetPaymentbyUser(string userid);
        Payment GetPayment(int id);
        void InserPayment(Payment model);
        void UpdatePayment(Payment model);
        IEnumerable<PaymentModel> PaymentView();

        void Save();
        void Dispose();

    }
    public class PaymentServices : IPaymentServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        readonly ICacheProviderService _cacheProviderService = new CacheProviderService();
        public IEnumerable<Payment> _getDatatrackingPayment()
        {
            // First, check the cache
            var trackingdata = _cacheProviderService.Get("TrackingPayment") as IDictionary<int, Payment>;

            // If it's not in the cache, we need to read it from the repository
            if (trackingdata == null)
            {
                //kiem tra thong bao trong ngay
                trackingdata = _unitOfWork.PaymentRepository.GetAll().ToDictionary(v => v.Id);

                if (trackingdata.Any())
                {
                    // Put this data into the cache for 300 minutes
                    _cacheProviderService.Set("TrackingPayment", trackingdata); // 300 minute
                }
            }
            return trackingdata.Values;
        }

        public IEnumerable<Payment> GetAllPayment()
        {
            return _getDatatrackingPayment();
        }
        public IEnumerable<Payment> GetPaymentbyUser(string userid)
        {
            return _unitOfWork.PaymentRepository.GetAll().Where(c=>c.UserId == userid);
        }
        public Payment GetPayment(int id)
        {
            return _getDatatrackingPayment().FirstOrDefault(c=>c.Id == id);
        }
        public void InserPayment(Payment model)
        {
            _unitOfWork.PaymentRepository.Insert(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingPayment");
            #endregion

        }
        public void UpdatePayment(Payment model)
        {
            _unitOfWork.PaymentRepository.Update(model);
            #region Clear cache when insert or update
            _cacheProviderService.Invalidate("TrackingPayment");
            #endregion

        }
        public IEnumerable<PaymentModel> PaymentView()
        {
            return _unitOfWork.PaymentViewRepository.ExecWithStoreProcedure("SELECT * FROM PaymentView");
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
