using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class UserRefMap : EntityTypeConfiguration<UserRef>
    {
       public UserRefMap()
       {
            //key
            HasKey(t => t.Id);
            //properties           
            Property(t => t.RefId).HasMaxLength(128).HasColumnType("nvarchar");
           Property(t => t.UserId).HasMaxLength(128).HasColumnType("nvarchar");

            //table
            ToTable("UserRefs");
       }       
    }
}
