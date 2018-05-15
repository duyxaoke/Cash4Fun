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
    public interface IBetServices
    {
        IEnumerable<Bet> GetAllBet();
        Bet GetBet(string resultRaceId, string userId, int animal1, int animal2);
        IEnumerable<Bet> GetBetbyUser(string resultRaceId, string userId);
        IEnumerable<Bet> GetAllBetbyUser(string userId);
        void InserBet(Bet model);
        void UpdateBet(Bet model);
        int CreateBetStore(string userid, string resultid, int animal1 = 0, int animal2 = 0, int coin1 = 0, int coin2 = 0);
        void Save();
        void Dispose();

    }
    public class BetServices : IBetServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<Bet> GetAllBet()
        {
            return _unitOfWork.BetRepository.GetAll();
        }
        public Bet GetBet(string resultRaceId, string userId, int animal1, int animal2)
        {
            return _unitOfWork.BetRepository.Get(c=>c.ResultRaceId == resultRaceId && c.UserId == userId && c.Animal1 == animal1 && c.Animal2 == animal2);
        }
        public IEnumerable<Bet> GetBetbyUser(string resultRaceId, string userId)
        {
            return _unitOfWork.BetRepository.GetMany(c => c.UserId == userId && c.ResultRaceId == resultRaceId);
        }
        public IEnumerable<Bet> GetAllBetbyUser(string userId)
        {
            return _unitOfWork.BetRepository.GetMany(c => c.UserId == userId).OrderByDescending(c=>c.CreateDate).Take(20);
        }
        public void InserBet(Bet model)
        {
            _unitOfWork.BetRepository.Insert(model);
        }
        public void UpdateBet(Bet model)
        {
            _unitOfWork.BetRepository.Update(model);
        }
        public int CreateBetStore(string userid, string resultid, int animal1 = 0, int animal2 = 0, int coin1 = 0, int coin2 = 0)
        {
            var result = _unitOfWork.ResultModelRepository.ExecWithStoreProcedure(String.Format("EXEC AcceptBet '{0}', '{1}', {2}, {3}, {4}, {5} ", userid, resultid, animal1, animal2, coin1, coin2)).FirstOrDefault().Value;
            return result;
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
