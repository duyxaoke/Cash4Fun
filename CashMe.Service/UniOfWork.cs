using CashMe.Core.Data;
using CashMe.Data.DAL;
using CashMe.Service.Models;
using CashMe.Shared.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace CashMe.Service
{
    public class UnitOfWork : IDisposable
    {
        #region Fields

        //public readonly CashMeContext Context = new CashMeContext();
        public readonly CashMeContext _context = new CashMeContext();
        //public readonly IdentityDbContext<ApplicationUser> _identityContext = new IdentityDbContext<ApplicationUser>;
        private BaseRepository<Message> _MessageRepository;
        private BaseRepository<MessageModel> _MessageViewRepository;
        private BaseRepository<Claims> _ClaimsRepository;
        private BaseRepository<Config> _ConfigRepository;
        private BaseRepository<Payment> _PaymentRepository;
        private BaseRepository<PaymentModel> _PaymentViewRepository;
        private BaseRepository<UserRef> _UserRefRepository;
        private BaseRepository<Wallet> _WalletRepository;
        private BaseRepository<UserInfo> _UserInfoRepository;
        private BaseRepository<Game> _GameRepository;
        private BaseRepository<UserRefModel> _UserRefViewRepository;
        private BaseRepository<UserViewModel> _UserViewRepository;
        private BaseRepository<Target> _TargetRepository;
        private BaseRepository<TargetMaster> _TargetMasterRepository;
        private BaseRepository<AnimalMaster> _AnimalMasterRepository;
        private BaseRepository<ResultRace> _ResultRaceRepository;
        private BaseRepository<ReportTOP> _ReportTOPRepository;
        private BaseRepository<ReportTOPModel> _reportTOPViewRepository;
        private BaseRepository<Bet> _betRepository;
        private BaseRepository<ResultModel> _resultModelRepository;
        private BaseRepository<UserInfoGameViewModel> _userInfoGameViewModelRepository;
        private BaseRepository<ChatMessageDetail> _chatMessageDetailRepository;
        private BaseRepository<ChatMessageDetailView> _chatMessageDetailViewRepository;
        private BaseRepository<ChatPrivateMessageDetail> _chatPrivateMessageDetailRepository;
        private BaseRepository<ChatUserDetail> _chatUserDetailRepository;
        private BaseRepository<ChatUserDetailView> _chatUserDetailViewRepository;
        private BaseRepository<IPlock> _IPlockRepository;
        private BaseRepository<MainViewModel> _mainViewRepository;

        private BaseRepository<IdentityRole> _identityRoleRepository;
        private BaseRepository<IdentityUser> _identityUserRepository;
        private BaseRepository<ApplicationUser> _applicationUserRepository;
        private BaseRepository<IdentityUserRole> _identityUserRoleRepository;


        #endregion

        #region Constructors and Destructors
        public BaseRepository<MainViewModel> MainViewRepository
        {
            get
            {
                if (this._mainViewRepository == null)
                    this._mainViewRepository = new BaseRepository<MainViewModel>(_context);
                return _mainViewRepository;
            }
        }
        public BaseRepository<IPlock> IPlockRepository
        {
            get
            {
                if (this._IPlockRepository == null)
                    this._IPlockRepository = new BaseRepository<IPlock>(_context);
                return _IPlockRepository;
            }
        }
        public BaseRepository<ChatUserDetailView> ChatUserDetailViewRepository
        {
            get
            {
                if (this._chatUserDetailViewRepository == null)
                    this._chatUserDetailViewRepository = new BaseRepository<ChatUserDetailView>(_context);
                return _chatUserDetailViewRepository;
            }
        }
        public BaseRepository<ChatMessageDetailView> ChatMessageDetailViewRepository
        {
            get
            {
                if (this._chatMessageDetailViewRepository == null)
                    this._chatMessageDetailViewRepository = new BaseRepository<ChatMessageDetailView>(_context);
                return _chatMessageDetailViewRepository;
            }
        }
        public BaseRepository<ChatUserDetail> ChatUserDetailRepository
        {
            get
            {
                if (this._chatUserDetailRepository == null)
                    this._chatUserDetailRepository = new BaseRepository<ChatUserDetail>(_context);
                return _chatUserDetailRepository;
            }
        }
        public BaseRepository<ChatPrivateMessageDetail> ChatPrivateMessageDetailRepository
        {
            get
            {
                if (this._chatPrivateMessageDetailRepository == null)
                    this._chatPrivateMessageDetailRepository = new BaseRepository<ChatPrivateMessageDetail>(_context);
                return _chatPrivateMessageDetailRepository;
            }
        }
        public BaseRepository<ChatMessageDetail> ChatMessageDetailRepository
        {
            get
            {
                if (this._chatMessageDetailRepository == null)
                    this._chatMessageDetailRepository = new BaseRepository<ChatMessageDetail>(_context);
                return _chatMessageDetailRepository;
            }
        }
        public BaseRepository<UserInfoGameViewModel> UserInfoGameViewModelRepository
        {
            get
            {
                if (this._userInfoGameViewModelRepository == null)
                    this._userInfoGameViewModelRepository = new BaseRepository<UserInfoGameViewModel>(_context);
                return _userInfoGameViewModelRepository;
            }
        }

        public BaseRepository<ResultModel> ResultModelRepository
        {
            get
            {
                if (this._resultModelRepository == null)
                    this._resultModelRepository = new BaseRepository<ResultModel>(_context);
                return _resultModelRepository;
            }
        }

        public BaseRepository<ReportTOPModel> ReportTOPViewRepository
        {
            get
            {
                if (this._reportTOPViewRepository == null)
                    this._reportTOPViewRepository = new BaseRepository<ReportTOPModel>(_context);
                return _reportTOPViewRepository;
            }
        }
        public BaseRepository<Bet> BetRepository
        {
            get
            {
                if (this._betRepository == null)
                    this._betRepository = new BaseRepository<Bet>(_context);
                return _betRepository;
            }
        }
        public BaseRepository<ReportTOP> ReportTOPRepository
        {
            get
            {
                if (this._ReportTOPRepository == null)
                    this._ReportTOPRepository = new BaseRepository<ReportTOP>(_context);
                return _ReportTOPRepository;
            }
        }
        public BaseRepository<ResultRace> ResultRaceRepository
        {
            get
            {
                if (this._ResultRaceRepository == null)
                    this._ResultRaceRepository = new BaseRepository<ResultRace>(_context);
                return _ResultRaceRepository;
            }
        }
        public BaseRepository<AnimalMaster> AnimalMasterRepository
        {
            get
            {
                if (this._AnimalMasterRepository == null)
                    this._AnimalMasterRepository = new BaseRepository<AnimalMaster>(_context);
                return _AnimalMasterRepository;
            }
        }
        public BaseRepository<Target> TargetRepository
        {
            get
            {
                if (this._TargetRepository == null)
                    this._TargetRepository = new BaseRepository<Target>(_context);
                return _TargetRepository;
            }
        }
        public BaseRepository<TargetMaster> TargetMasterRepository
        {
            get
            {
                if (this._TargetMasterRepository == null)
                    this._TargetMasterRepository = new BaseRepository<TargetMaster>(_context);
                return _TargetMasterRepository;
            }
        }
        public BaseRepository<Game> GameRepository
        {
            get
            {
                if (this._GameRepository == null)
                    this._GameRepository = new BaseRepository<Game>(_context);
                return _GameRepository;
            }
        }

        public BaseRepository<UserInfo> UserInfoRepository
        {
            get
            {
                if (this._UserInfoRepository == null)
                    this._UserInfoRepository = new BaseRepository<UserInfo>(_context);
                return _UserInfoRepository;
            }
        }

        public BaseRepository<Claims> ClaimsRepository
        {
            get
            {
                if (this._ClaimsRepository == null)
                    this._ClaimsRepository = new BaseRepository<Claims>(_context);
                return _ClaimsRepository;
            }
        }

        public BaseRepository<Config> ConfigRepository
        {
            get
            {
                if (this._ConfigRepository == null)
                    this._ConfigRepository = new BaseRepository<Config>(_context);
                return _ConfigRepository;
            }
        }
        public BaseRepository<Payment> PaymentRepository
        {
            get
            {
                if (this._PaymentRepository == null)
                    this._PaymentRepository = new BaseRepository<Payment>(_context);
                return _PaymentRepository;
            }
        }
        public BaseRepository<PaymentModel> PaymentViewRepository
        {
            get
            {
                if (this._PaymentViewRepository == null)
                    this._PaymentViewRepository = new BaseRepository<PaymentModel>(_context);
                return _PaymentViewRepository;
            }
        }

        public BaseRepository<UserRef> UserRefRepository
        {
            get
            {
                if (this._UserRefRepository == null)
                    this._UserRefRepository = new BaseRepository<UserRef>(_context);
                return _UserRefRepository;
            }
        }
        public BaseRepository<Wallet> WalletRepository
        {
            get
            {
                if (this._WalletRepository == null)
                    this._WalletRepository = new BaseRepository<Wallet>(_context);
                return _WalletRepository;
            }
        }
        public BaseRepository<ApplicationUser> ApplicationUserRepository
        {
            get
            {
                if (this._applicationUserRepository == null)
                    this._applicationUserRepository = new BaseRepository<ApplicationUser>(_context);
                return _applicationUserRepository;
            }
        }
        public BaseRepository<IdentityRole> IdentityRoleRepository
        {            
            get
            {
                if (this._identityRoleRepository == null)
                    this._identityRoleRepository = new BaseRepository<IdentityRole>(_context);
                return _identityRoleRepository;
            }
        }
        public BaseRepository<IdentityUser> IdentityUserRepository
        {
            get
            {
                if (this._identityUserRepository == null)
                    this._identityUserRepository = new BaseRepository<IdentityUser>(_context);
                return _identityUserRepository;
            }
        }
        public BaseRepository<IdentityUserRole> IdentityUserRoleRepository
        {
            get
            {
                if (this._identityUserRoleRepository == null)
                    this._identityUserRoleRepository = new BaseRepository<IdentityUserRole>(_context);
                return _identityUserRoleRepository;
            }
        }
        public BaseRepository<Message> MessageRepository
        {
            get
            {
                if (this._MessageRepository == null)
                    this._MessageRepository = new BaseRepository<Message>(_context);
                return _MessageRepository;
            }
        }

        public BaseRepository<MessageModel> MessageViewRepository
        {
            get
            {
                if (this._MessageViewRepository == null)
                    this._MessageViewRepository = new BaseRepository<MessageModel>(_context);
                return _MessageViewRepository;
            }
        }
        public BaseRepository<UserRefModel> UserRefViewRepository
        {
            get
            {
                if (this._UserRefViewRepository == null)
                    this._UserRefViewRepository = new BaseRepository<UserRefModel>(_context);
                return _UserRefViewRepository;
            }
        }
        public BaseRepository<UserViewModel> UserViewRepository
        {
            get
            {
                if (this._UserViewRepository == null)
                    this._UserViewRepository = new BaseRepository<UserViewModel>(_context);
                return _UserViewRepository;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Save()
        {
            _context.BulkSaveChanges();
        }
        #endregion

        #region Disposed

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
        #endregion
    }
}
