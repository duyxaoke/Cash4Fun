using CashMe.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CashMe.Data.Mapping
{
   public class ConfigMap : EntityTypeConfiguration<Config> 
    {
       public ConfigMap()
       {
            //key
            HasKey(t => t.Id);
            //properties
            Property(t => t.GiaCoin).IsRequired();
            Property(t => t.HoaHong).IsRequired();
            Property(t => t.ImageForNewMember).IsRequired();
            Property(t => t.Version).HasMaxLength(20).HasColumnType("nvarchar");
            //table
            ToTable("Config");
       }
    }
}
