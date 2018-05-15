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
    public interface IUserInfoServices
    {
        IEnumerable<UserInfo> GetAllUserInfo();
        IEnumerable<UserInfo> GetUserInfobyWalletActive();
        IEnumerable<UserViewModel> UserView();
        UserInfo GetUserInfo(int id);
        UserInfo GetUserInfobyUser(string userid);
        void InserUserInfo(UserInfo model);
        void UpdateUserInfo(UserInfo model);
        UserInfo GetUserInfobyWallet(int walletId);
        IEnumerable<UserViewModel> getUserInfoByStore(string userid);
        UserInfoGameViewModel UserInfoGameView(string userid);
        void UpdateWalletUserInfoByStore();
        void AddClaimApp(int Coin, string UserId);
        UserInfo CheckWalletUserInfo(string userid);
        void Save();
        void Dispose();

    }
    public class UserInfoServices : IUserInfoServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<UserViewModel> UserView()
        {
            return _unitOfWork.UserViewRepository.ExecWithStoreProcedure("SELECT TOP 50 * FROM UserView order by CountImage desc");
        }
        public IEnumerable<UserInfo> GetAllUserInfo()
        {
            return _unitOfWork.UserInfoRepository.GetAll();
        }
        public IEnumerable<UserInfo> GetUserInfobyWalletActive()
        {
            return _unitOfWork.UserInfoRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM UserInfoes WITH(NOLOCK) WHERE WalletId <> 0"));
        }
        public UserInfo GetUserInfo(int id)
        {
            return _unitOfWork.UserInfoRepository.GetById(id);
        }
        public UserInfo GetUserInfobyUser(string userid)
        {
            return _unitOfWork.UserInfoRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM UserInfoes WITH(NOLOCK) WHERE UserId = '{0}'", userid)).FirstOrDefault();
        }
        public UserInfo GetUserInfobyWallet(int walletId)
        {
            return _unitOfWork.UserInfoRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM UserInfoes WITH(NOLOCK) WHERE WalletId = {0}", walletId)).FirstOrDefault();
        }
        public void InserUserInfo(UserInfo model)
        {
            _unitOfWork.UserInfoRepository.Insert(model);
        }
        public void UpdateUserInfo(UserInfo model)
        {
            _unitOfWork.UserInfoRepository.Update(model);
        }
        public IEnumerable<UserViewModel> getUserInfoByStore(string userid)
        {
            return _unitOfWork.UserViewRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM UserView WITH(NOLOCK) WHERE Id = '{0}'", userid));

        }
        public void UpdateWalletUserInfoByStore()
        {
            _unitOfWork.UserViewRepository.ExecStore("EXEC UpdateUser_30Minute");

        }
        public void AddClaimApp(int Coin, string UserId)
        {
            _unitOfWork.UserViewRepository.ExecStore(String.Format("EXEC AddClaimApp {0}, '{1}'", Coin, UserId));

        }

        public UserInfoGameViewModel UserInfoGameView(string userid)
        {
            return _unitOfWork.UserInfoGameViewModelRepository.ExecWithStoreProcedure(String.Format("SELECT * FROM UserInfoGameView WITH(NOLOCK) WHERE UserId = '{0}'", userid)).FirstOrDefault();
        }
        public UserInfo CheckWalletUserInfo(string userid)
        {
            return _unitOfWork.UserInfoRepository.ExecWithStoreProcedure(String.Format("EXEC GetUserInfo '{0}'", userid)).FirstOrDefault();
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
