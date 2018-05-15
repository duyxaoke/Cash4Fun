using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class TargetMasterMap : EntityTypeConfiguration<TargetMaster>
    {
       public TargetMasterMap()
       {
            //key
            HasKey(t => t.Id);
            //Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Color).HasMaxLength(20).HasColumnType("nvarchar");

            //table
            ToTable("TargetMaster");
       }       
    }
}
