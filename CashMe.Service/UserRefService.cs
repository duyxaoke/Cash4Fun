using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
namespace CashMe.Service
{
    public interface IUserRefServices
    {
        IEnumerable<UserRef> GetAllUserRef();
        UserRef GetUserRef(string id);
        IEnumerable<UserRef> GetUserRefbyUser(string id);
        IEnumerable<UserRefModel> GetUserRefView(string id);
        void InserUserRef(UserRef model);
        void UpdateUserRef(UserRef model);

        void Save();
        void Dispose();

    }
    public class UserRefServices : IUserRefServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<UserRef> GetAllUserRef()
        {
            return _unitOfWork.UserRefRepository.GetAll();
        }
        public UserRef GetUserRef(string id)
        {
            return _unitOfWork.UserRefRepository.GetById(id);
        }
        public IEnumerable<UserRef> GetUserRefbyUser(string id)
        {
            return _unitOfWork.UserRefRepository.GetMany(c => c.UserId == id);
        }
        public IEnumerable<UserRefModel> GetUserRefView(string id)
        {
            return _unitOfWork.UserRefViewRepository.ExecWithStoreProcedure("EXEC UserRefView '"+ id + "'");
        }
        public void InserUserRef(UserRef model)
        {
            _unitOfWork.UserRefRepository.Insert(model);
        }
        public void UpdateUserRef(UserRef model)
        {
            _unitOfWork.UserRefRepository.Update(model);
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
