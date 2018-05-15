using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using CashMe.Service;
using CashMe.Core.Data;
using Microsoft.AspNet.Identity;

namespace CashMe.Admin.Hubs
{
    public class ChatHub : Hub
    {
        private ChatUserDetailServices _chatUserDetailServices = new ChatUserDetailServices();
        private ChatMessageDetailServices _chatMessageDetailServices = new ChatMessageDetailServices();
        private ChatPrivateMessageDetailServices _chatPrivateMessageDetailServices = new ChatPrivateMessageDetailServices();
        private UserInfoServices _userInfoServices = new UserInfoServices();
        private IPlockServices _IPlockServices = new IPlockServices();


        #region Connect
        public void Connect()
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            string UserName = System.Web.HttpContext.Current.User.Identity.GetUserName();
            var userInfo = _userInfoServices.GetUserInfobyUser(UserId);

            var id = Context.ConnectionId;
            var item = _chatUserDetailServices.GetChatUserDetailByUser(UserId);
                if (item != null)
                {
                _chatUserDetailServices.DeleteChatUserDetail(item);
                 // Disconnect
                 Clients.All.onUserDisconnectedExisting(item.ConnectionId, item.UserId);
                }

                var Users = _chatUserDetailServices.GetAllChatUserDetail();
                if (Users.Where(x => x.UserId == UserId).ToList().Count == 0)
                {
                    var userdetails = new ChatUserDetail
                    {
                        Id = Guid.NewGuid(),
                        ConnectionId = id,
                        UserId = UserId,
                        UserName = UserName
                    };
                _chatUserDetailServices.InserChatUserDetail(userdetails);

                }
            // send to caller
            var connectedUsers = _chatUserDetailServices.GetAllChatUserDetailView();
            var CurrentMessage = _chatMessageDetailServices.GetAllChatMessageDetailView().OrderBy(c=>c.CreateDate);
            Clients.Caller.onConnected(id, UserId, UserName, connectedUsers, CurrentMessage);

            // send to all except caller client
            Clients.AllExcept(id).onNewUserConnected(id, UserId, UserName, userInfo.ImageRank, userInfo.TextColor);
        }
        #endregion

        #region Disconnect
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = _chatUserDetailServices.GetChatUserDetailByConnectionId(Context.ConnectionId);
            if (item != null)
            {
                _chatUserDetailServices.DeleteChatUserDetail(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserId, item.UserName);
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region Send_To_All
        public void SendMessageToAll(string message)
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            string UserName = System.Web.HttpContext.Current.User.Identity.GetUserName();
            var userInfo = _userInfoServices.GetUserInfobyUser(UserId);
            string IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            if (userInfo.IsActive == true && _IPlockServices.CheckIP(IP))
            {
                // store last 100 messages in cache
                AddAllMessageinCache(UserId, UserName, message);

                // Broad cast message
                Clients.All.messageReceived(UserId, UserName, message, userInfo.ImageRank, userInfo.TextColor);
            }
        }
        #endregion


        #region Save_Cache
        private void AddAllMessageinCache(string UserId, string UserName, string message)
        {
                var messageDetail = new ChatMessageDetail
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    UserId = UserId,
                    UserName = UserName,
                    CreateDate = DateTime.Now
                };
                _chatMessageDetailServices.InserChatMessageDetail(messageDetail);
        }

        #endregion
    }

    public class PrivateChatMessage
    {
        public string userName { get; set; }
        public string message { get; set; }
    }
}