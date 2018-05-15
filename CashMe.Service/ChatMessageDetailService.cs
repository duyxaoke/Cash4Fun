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
    public interface IChatMessageDetailServices
    {
        IEnumerable<ChatMessageDetail> GetAllChatMessageDetail();
        IEnumerable<ChatMessageDetailView> GetAllChatMessageDetailView();
        ChatMessageDetail GetChatMessageDetail(string id);
        void InserChatMessageDetail(ChatMessageDetail model);
        void UpdateChatMessageDetail(ChatMessageDetail model);

        void Save();
        void Dispose();

    }
    public class ChatMessageDetailServices : IChatMessageDetailServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ChatMessageDetail> GetAllChatMessageDetail()
        {
            return _unitOfWork.ChatMessageDetailRepository.GetAll();
        }
        public IEnumerable<ChatMessageDetailView> GetAllChatMessageDetailView()
        {
            return _unitOfWork.ChatMessageDetailViewRepository.ExecWithStoreProcedure("SELECT TOP 100 * FROM ChatMessageDetailView order by CreateDate desc");
        }
        public ChatMessageDetail GetChatMessageDetail(string id)
        {
            return _unitOfWork.ChatMessageDetailRepository.GetById(id);
        }
        public void InserChatMessageDetail(ChatMessageDetail model)
        {
            _unitOfWork.ChatMessageDetailRepository.Insert(model);

        }
        public void UpdateChatMessageDetail(ChatMessageDetail model)
        {
            _unitOfWork.ChatMessageDetailRepository.Update(model);
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
