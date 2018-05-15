using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class ResultRaceMap : EntityTypeConfiguration<ResultRace>
    {
       public ResultRaceMap()
       {
            //key
            HasKey(t => t.Id);
            //Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           //table
           ToTable("ResultRace");
       }       
    }
}
