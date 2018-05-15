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
    public interface ITargetServices
    {
        IEnumerable<Target> GetAllTarget();
        Target GetTarget(int id);
        Target GetTargetbyUser(string userid);
        IEnumerable<Target> GetTargetHasValue();
        void InserTarget(Target model);
        void UpdateTarget(Target model);
        void ResetTarget();

        void Save();
        void Dispose();

    }
    public class TargetServices : ITargetServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<Target> GetAllTarget()
        {
            return _unitOfWork.TargetRepository.GetAll();
        }
        public Target GetTarget(int id)
        {
            return _unitOfWork.TargetRepository.GetById(id);
        }
        public Target GetTargetbyUser(string userid)
        {
            return _unitOfWork.TargetRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM Target WITH(NOLOCK) WHERE UserId = '{0}'", userid)).FirstOrDefault();
        }
        public IEnumerable<Target> GetTargetHasValue()
        {
            return _unitOfWork.TargetRepository.ExecWithStoreProcedure("SELECT * FROM Target WITH(NOLOCK) WHERE CountImage <> 0");
        }
        public void InserTarget(Target model)
        {
            _unitOfWork.TargetRepository.Insert(model);
        }
        public void UpdateTarget(Target model)
        {
            _unitOfWork.TargetRepository.Update(model);
        }
        public void ResetTarget()
        {
            _unitOfWork.TargetRepository.ExecStore("EXEC Reset_Target");
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
