using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class ReportTOPMap : EntityTypeConfiguration<ReportTOP>
    {
       public ReportTOPMap()
       {
            //key
            HasKey(t => t.Id);
            //Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties           
            Property(t => t.UserId).HasMaxLength(128).HasColumnType("nvarchar");
           Property(t => t.CountImage).IsRequired();
           //table
           ToTable("ReportTOP");
       }       
    }
}
