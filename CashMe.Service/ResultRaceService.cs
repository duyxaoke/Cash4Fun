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
    public interface IResultRaceServices
    {
        IEnumerable<ResultRace> GetAllResultRace();
        ResultRace GetResultRace(string id);
        void InserResultRace(ResultRace model);
        void UpdateResultRace(ResultRace model);
        void InserResultRaceStore(string Guild);
        void UpdateResultRaceStore(string guid, int top1, int top2, int top3, int top4, int top5, int top6, int top7, int top8, int top9, int top10, int top11, int top12);
        int Final_ResultRace(string lsResult, string userid, string resultraceid);
        void Save();
        void Dispose();

    }
    public class ResultRaceServices : IResultRaceServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ResultRace> GetAllResultRace()
        {
            return _unitOfWork.ResultRaceRepository.GetAll();
        }
        public ResultRace GetResultRace(string id)
        {
            return _unitOfWork.ResultRaceRepository.Get(c=>c.Id.ToString() == id);
        }
        public void InserResultRaceStore(string Guild)
        {
            _unitOfWork.ResultRaceRepository.ExecStore(String.Format("EXEC CreateResultRace '{0}'", Guild));

        }
        public void InserResultRace(ResultRace model)
        {
            _unitOfWork.ResultRaceRepository.Insert(model);
        }
        public void UpdateResultRace(ResultRace model)
        {
            _unitOfWork.ResultRaceRepository.Update(model);
        }
        public void UpdateResultRaceStore(string guid, int top1, int top2, int top3, int top4, int top5, int top6, int top7, int top8, int top9, int top10, int top11, int top12)
        {
            _unitOfWork.ResultModelRepository.ExecStore(String.Format("EXEC UpdateResultRace '{0}',{1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}", guid, top1, top2, top3, top4, top5, top6, top7, top8, top9, top10, top11, top12));
        }
        public int Final_ResultRace(string lsResult, string userid, string resultraceid)
        {
            return _unitOfWork.ResultModelRepository.ExecWithStoreProcedure(String.Format("EXEC Final_ResultRace '{0}','{1}', '{2}'", lsResult, userid, resultraceid)).FirstOrDefault().Value;
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
