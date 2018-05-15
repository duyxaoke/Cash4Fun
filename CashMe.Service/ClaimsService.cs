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
    public interface IClaimsServices
    {
        IEnumerable<Claims> GetAllClaims();
        Claims GetClaims(int id);
        Claims GetClaimsbyUser(string userid);
        void InserClaims(Claims model);
        void UpdateClaims(Claims model);
        void UpdateClaims_1Minute(string userid, string code, int asktillnow, int claim);
        void UpdateClaims_30Minute(string userid, int walletid, int asktillnow, int claim, int statusupdate);
        void Save();
        void Dispose();

    }
    public class ClaimsServices : IClaimsServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<Claims> GetAllClaims()
        {
            return _unitOfWork.ClaimsRepository.GetAll();
        }
        public Claims GetClaims(int id)
        {
            return _unitOfWork.ClaimsRepository.GetById(id);
        }
        public Claims GetClaimsbyUser(string userid)
        {
            return _unitOfWork.ClaimsRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM Claims WITH(NOLOCK) WHERE UserId = '{0}'", userid)).FirstOrDefault();
        }
        public void InserClaims(Claims model)
        {
            _unitOfWork.ClaimsRepository.Insert(model);
        }
        public void UpdateClaims(Claims model)
        {
            _unitOfWork.ClaimsRepository.Update(model);
        }
        public void UpdateClaims_1Minute(string userid, string code, int asktillnow, int claim)
        {
            _unitOfWork.ClaimsRepository.ExecStore(String.Format("EXEC UpdateClaims_1Minute '{0}','{1}',{2},{3}", userid, code, asktillnow, claim));
        }
        public void UpdateClaims_30Minute(string userid, int walletid, int asktillnow, int claim, int statusupdate)
        {
            _unitOfWork.ClaimsRepository.ExecStore(String.Format("EXEC UpdateClaims_30Minute '{0}',{1},{2},{3},{4}", userid, walletid, asktillnow, claim, statusupdate));
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
