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
    public interface IChatUserDetailServices
    {
        IEnumerable<ChatUserDetail> GetAllChatUserDetail();
        IEnumerable<ChatUserDetailView> GetAllChatUserDetailView();
        ChatUserDetail GetChatUserDetail(string id);
        ChatUserDetail GetChatUserDetailByUser(string userid);
        ChatUserDetail GetChatUserDetailByConnectionId(string ConnectionId);
        void InserChatUserDetail(ChatUserDetail model);
        void UpdateChatUserDetail(ChatUserDetail model);
        void DeleteChatUserDetail(ChatUserDetail model);
        void Save();
        void Dispose();

    }
    public class ChatUserDetailServices : IChatUserDetailServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ChatUserDetail> GetAllChatUserDetail()
        {
            return _unitOfWork.ChatUserDetailRepository.GetAll();
        }
        public IEnumerable<ChatUserDetailView> GetAllChatUserDetailView()
        {
            return _unitOfWork.ChatUserDetailViewRepository.ExecWithStoreProcedure("SELECT * FROM ChatUserDetailView");
        }
        public ChatUserDetail GetChatUserDetail(string id)
        {
            return _unitOfWork.ChatUserDetailRepository.GetById(id);
        }
        public ChatUserDetail GetChatUserDetailByUser(string userid)
        {
            return _unitOfWork.ChatUserDetailRepository.Get(c => c.UserId == userid);
        }
        public ChatUserDetail GetChatUserDetailByConnectionId(string ConnectionId)
        {
            return _unitOfWork.ChatUserDetailRepository.Get(c => c.ConnectionId == ConnectionId);
        }
        public void InserChatUserDetail(ChatUserDetail model)
        {
            _unitOfWork.ChatUserDetailRepository.Insert(model);

        }
        public void UpdateChatUserDetail(ChatUserDetail model)
        {
            _unitOfWork.ChatUserDetailRepository.Update(model);
        }
        public void DeleteChatUserDetail(ChatUserDetail model)
        {
            _unitOfWork.ChatUserDetailRepository.Delete(model);
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
