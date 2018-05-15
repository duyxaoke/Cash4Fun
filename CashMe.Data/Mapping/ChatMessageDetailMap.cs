using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class ChatMessageDetailMap : EntityTypeConfiguration<ChatMessageDetail>
    {
       public ChatMessageDetailMap()
       {
            //key
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties           
            Property(t => t.UserId).HasMaxLength(128).HasColumnType("nvarchar");
            Property(t => t.UserName).HasMaxLength(128).HasColumnType("nvarchar");
            Property(t => t.Message).HasMaxLength(500).HasColumnType("nvarchar");
            //table
            ToTable("ChatMessageDetail");
       }       
    }
}
