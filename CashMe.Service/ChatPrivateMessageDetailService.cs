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
    public interface IChatPrivateMessageDetailServices
    {
        IEnumerable<ChatPrivateMessageDetail> GetAllChatPrivateMessageDetail();
        ChatPrivateMessageDetail GetChatPrivateMessageDetail(string id);
        void InserChatPrivateMessageDetail(ChatPrivateMessageDetail model);
        void UpdateChatPrivateMessageDetail(ChatPrivateMessageDetail model);

        void Save();
        void Dispose();

    }
    public class ChatPrivateMessageDetailServices : IChatPrivateMessageDetailServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ChatPrivateMessageDetail> GetAllChatPrivateMessageDetail()
        {
            return _unitOfWork.ChatPrivateMessageDetailRepository.GetAll();
        }
        public ChatPrivateMessageDetail GetChatPrivateMessageDetail(string id)
        {
            return _unitOfWork.ChatPrivateMessageDetailRepository.GetById(id);
        }
        public void InserChatPrivateMessageDetail(ChatPrivateMessageDetail model)
        {
            _unitOfWork.ChatPrivateMessageDetailRepository.Insert(model);

        }
        public void UpdateChatPrivateMessageDetail(ChatPrivateMessageDetail model)
        {
            _unitOfWork.ChatPrivateMessageDetailRepository.Update(model);
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
