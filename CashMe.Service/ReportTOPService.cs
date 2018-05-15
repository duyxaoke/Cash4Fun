using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace CashMe.Service
{
    public interface IReportTOPServices
    {
        IEnumerable<ReportTOP> GetAllReportTOP();
        ReportTOP GetReportTOP(int id);
        ReportTOP GetReportTOPbyUser(string userid);
        void InserReportTOP(ReportTOP model);
        void UpdateReportTOP(ReportTOP model);
        IEnumerable<ReportTOPModel> ReportTOP_Week();
        IEnumerable<ReportTOPModel> ReportTOP_Month();

        void Save();
        void Dispose();

    }
    public class ReportTOPServices : IReportTOPServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ReportTOP> GetAllReportTOP()
        {
            return _unitOfWork.ReportTOPRepository.GetAll();
        }
        public ReportTOP GetReportTOP(int id)
        {
            return _unitOfWork.ReportTOPRepository.GetById(id);
        }
        public ReportTOP GetReportTOPbyUser(string userid)
        {
            return _unitOfWork.ReportTOPRepository.Get(c=>c.UserId == userid);
        }
        public void InserReportTOP(ReportTOP model)
        {
            _unitOfWork.ReportTOPRepository.Insert(model);
        }
        public void UpdateReportTOP(ReportTOP model)
        {
            _unitOfWork.ReportTOPRepository.Update(model);
        }
        public IEnumerable<ReportTOPModel> ReportTOP_Week()
        {
            return _unitOfWork.ReportTOPViewRepository.ExecWithStoreProcedure("SELECT * FROM ReportTOP_Week");
        }
        public IEnumerable<ReportTOPModel> ReportTOP_Month()
        {
            return _unitOfWork.ReportTOPViewRepository.ExecWithStoreProcedure("SELECT * FROM ReportTOP_Month");
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
