using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class ChatPrivateMessageDetailMap : EntityTypeConfiguration<ChatPrivateMessageDetail>
    {
       public ChatPrivateMessageDetailMap()
       {
            //key
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties           
            Property(t => t.MasterUserId).HasMaxLength(128).HasColumnType("nvarchar");
            Property(t => t.ChatToUserId).HasMaxLength(128).HasColumnType("nvarchar");
            Property(t => t.Message).HasMaxLength(500).HasColumnType("nvarchar");
            //table
            ToTable("ChatPrivateMessageDetail");
       }       
    }
}
