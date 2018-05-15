using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using System;
using System.Linq;

namespace CashMe.Service
{
    public interface IIPlockServices
    {
        IEnumerable<IPlock> GetAllIPlock();
        bool CheckIP(string IP);
        void InserIPlock(IPlock model);
        void UpdateIPlock(IPlock model);
        void Save();
        void Dispose();

    }
    public class IPlockServices : IIPlockServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<IPlock> GetAllIPlock()
        {
            return _unitOfWork.IPlockRepository.GetAll();
        }
        public bool CheckIP(string IP)
        {
            var result = _unitOfWork.IPlockRepository.Get(c=>c.IP == IP);
            return result == null ? true : false;
        }
        public void InserIPlock(IPlock model)
        {
            _unitOfWork.IPlockRepository.Insert(model);
        }
        public void UpdateIPlock(IPlock model)
        {
            _unitOfWork.IPlockRepository.Update(model);
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
