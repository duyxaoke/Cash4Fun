using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class ReportClaimsMap : EntityTypeConfiguration<ReportClaims>
    {
       public ReportClaimsMap()
       {
            //key
            HasKey(t => t.Id);
            //Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties           
           //table
           ToTable("ReportClaims");
       }       
    }
}
