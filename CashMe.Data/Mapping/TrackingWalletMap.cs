using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class TrackingWalletMap : EntityTypeConfiguration<TrackingWallet>
    {
       public TrackingWalletMap()
       {
            //key
            HasKey(t => t.Id);
            //properties           
            //table
            ToTable("TrackingWallet");
       }       
    }
}
