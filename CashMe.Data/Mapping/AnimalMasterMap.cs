using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class AnimalMasterMap : EntityTypeConfiguration<AnimalMaster>
    {
       public AnimalMasterMap()
       {
            //key
            HasKey(t => t.Id);
            //Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties           
           Property(t => t.Key).HasMaxLength(128).HasColumnType("nvarchar");
            //table
            ToTable("AnimalMaster");
       }       
    }
}
