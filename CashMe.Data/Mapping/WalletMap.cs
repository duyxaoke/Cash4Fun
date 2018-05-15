using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class WalletMap : EntityTypeConfiguration<Wallet>
    {
       public WalletMap()
       {
            //key
            HasKey(t => t.Id);
            //properties           
            Property(t => t.Claim).IsRequired();
           Property(t => t.Code).HasMaxLength(100).HasColumnType("nvarchar");
            Property(t => t.Status).IsRequired();
            Property(t => t.UpdateDate).IsRequired();
            //table
            ToTable("Wallets");
       }       
    }
}
