
using CashMe.Core.Data;
using CashMe.Data.Mapping;
using CashMe.Shared.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CashMe.Data.DAL
{
    public class CashMeContext : IdentityDbContext<ApplicationUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public CashMeContext() : base("name=DbConnect")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CashMeContext, CashMeInitializer>());//initial database use test data            
        }

        public static CashMeContext Create()
        {
            return new CashMeContext();
        }

        //DbContext
        public DbSet<Message> Message { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<Claims> Claims { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<UserRef> UserRef { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<Target> Target { get; set; }
        public DbSet<TargetMaster> TargetMaster { get; set; }
        public DbSet<ReportClaims> ReportClaims { get; set; }
        public DbSet<AnimalMaster> AnimalMaster { get; set; }
        public DbSet<Bet> Bet { get; set; }
        public DbSet<ResultRace> ResultRace { get; set; }
        public DbSet<ReportTOP> ReportTOP { get; set; }
        public DbSet<TrackingWallet> TrackingWallet { get; set; }
        public DbSet<ChatMessageDetail> ChatMessageDetail { get; set; }
        public DbSet<ChatPrivateMessageDetail> ChatPrivateMessageDetail { get; set; }
        public DbSet<ChatUserDetail> ChatUserDetail { get; set; }
        public DbSet<IPlock> IPlock { get; set; }

        //Auth

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new ClaimsMap());
            modelBuilder.Configurations.Add(new ConfigMap());
            modelBuilder.Configurations.Add(new PaymentMap());
            modelBuilder.Configurations.Add(new UserInfoMap());
            modelBuilder.Configurations.Add(new UserRefMap());
            modelBuilder.Configurations.Add(new WalletMap());
            modelBuilder.Configurations.Add(new GameMap());
            modelBuilder.Configurations.Add(new TargetMap());
            modelBuilder.Configurations.Add(new TargetMasterMap());
            modelBuilder.Configurations.Add(new ReportClaimsMap());
            modelBuilder.Configurations.Add(new AnimalMasterMap());
            modelBuilder.Configurations.Add(new BetMap());
            modelBuilder.Configurations.Add(new ResultRaceMap());
            modelBuilder.Configurations.Add(new ReportTOPMap());
            modelBuilder.Configurations.Add(new TrackingWalletMap());
            modelBuilder.Configurations.Add(new ChatMessageDetailMap());
            modelBuilder.Configurations.Add(new ChatPrivateMessageDetailMap());
            modelBuilder.Configurations.Add(new ChatUserDetailMap());
            modelBuilder.Configurations.Add(new IPlockMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
