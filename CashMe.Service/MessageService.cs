using System.Collections.Generic;
using CashMe.Core.Data;
using CashMe.Service.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace CashMe.Service
{
    public interface IMessageService
    {
        IEnumerable<MessageModel> GetMessageView();
        Message GetMessage(long id);
        void InsertMessage(MessageModel model);
        void UpdateMessage(Message model);
        void Save();
        void Dispose();

    }
    public class MessageService : IMessageService
    {
        readonly UnitOfWork unitOfWork = new UnitOfWork();
        public IEnumerable<MessageModel> GetMessageView()
       {
            return unitOfWork.MessageViewRepository.ExecWithStoreProcedure("SELECT * FROM MessageView");
        }
        public Message GetMessage(long id)
       {
           return unitOfWork.MessageRepository.GetById(id);
       }
        public void InsertMessage(MessageModel model)
       {
            var ListRoles = model.ListRoles;
            var ListUsers = model.ListUsers;

            #region Kiểm tra và gửi thông tin cho Roles được chọn
            if (ListRoles != null)
            {
                using (var transaction = new TransactionScope())
                {
                    var ListIdRolestArray = ListRoles;
                    var ListUsersOfRole = unitOfWork.IdentityUserRoleRepository.GetMany(p => ListIdRolestArray.Contains(p.RoleId));
                    foreach (var item in ListUsersOfRole)
                    {
                        var ms = new Message
                        {
                            Title = model.Title,
                            Body = model.Body,
                            FromUserId = model.FromUserId,
                            ToUserId = item.UserId,
                            Status = model.Status,
                            CreateDate = model.CreateDate
                        };
                        unitOfWork.MessageRepository.Insert(ms);
                    }
                    transaction.Complete();
                }
            }
            #endregion

            #region Kiểm tra và gửi thông tin cho users được chọn
            if (ListUsers != null)
            {
                using (var transaction = new TransactionScope())
                {
                    var ListIdUserstArray = ListUsers;
                    var ListUsersSelect = unitOfWork.ApplicationUserRepository.GetMany(p => ListIdUserstArray.Contains(p.Id));
                    foreach (var item in ListUsersSelect)
                    {
                        var ms = new Message
                        {
                            Title = model.Title,
                            Body = model.Body,
                            FromUserId = model.FromUserId,
                            ToUserId = item.Id,
                            Status = model.Status,
                            CreateDate = model.CreateDate
                        };
                        unitOfWork.MessageRepository.Insert(ms);
                    }
                    transaction.Complete();
                }
            }
            #endregion

        }
        public void UpdateMessage(Message model)
        {
            unitOfWork.MessageRepository.Update(model);
        }

        public void Save()
        {
            unitOfWork.Save();
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
        }

    }
}
