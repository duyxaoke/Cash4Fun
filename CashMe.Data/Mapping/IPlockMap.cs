using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class IPlockMap : EntityTypeConfiguration<IPlock>
    {
       public IPlockMap()
       {
            //key
            HasKey(t => t.Id);
            //Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties           
            Property(t => t.IP).HasMaxLength(20).HasColumnType("nvarchar");
           //table
           ToTable("IPlock");
       }       
    }
}
