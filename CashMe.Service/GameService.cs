using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace CashMe.Service
{
    public interface IGameService
    {
        IEnumerable<Game> GetAllGame();
        Game GetGame(int id);
        IEnumerable<Game> GetGamebyUser(string userid);
        void InserGame(Game model);
        void UpdateGame(Game model);

        void Save();
        void Dispose();

    }
    public class GameService : IGameService
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<Game> GetAllGame()
        {
            return _unitOfWork.GameRepository.GetAll();
        }
        public Game GetGame(int id)
        {
            return _unitOfWork.GameRepository.GetById(id);
        }
        public IEnumerable<Game> GetGamebyUser(string userid)
        {
            return _unitOfWork.GameRepository.GetMany(c=>c.UserId == userid);
        }
        public void InserGame(Game model)
        {
            _unitOfWork.GameRepository.Insert(model);
        }
        public void UpdateGame(Game model)
        {
            _unitOfWork.GameRepository.Update(model);
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
