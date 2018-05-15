using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class UserInfoMap : EntityTypeConfiguration<UserInfo>
    {
       public UserInfoMap()
       {
            //key
            HasKey(t => t.Id);
            //properties           
            Property(t => t.Amount).IsRequired();
           Property(t => t.CreateDate).IsRequired();
            Property(t => t.UserId).HasMaxLength(128).HasColumnType("nvarchar");
            Property(t => t.IP).HasMaxLength(20).HasColumnType("nvarchar");
            Property(t => t.ImageRank).HasMaxLength(100).HasColumnType("nvarchar");
            Property(t => t.TextColor).HasMaxLength(20).HasColumnType("nvarchar");
            Property(t => t.Password).HasMaxLength(100).HasColumnType("nvarchar");
            Property(t => t.WalletId).IsRequired();
            //table
            ToTable("UserInfoes");
       }       
    }
}
