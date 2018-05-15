using System.Data.Entity.ModelConfiguration;
using CashMe.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMe.Data.Mapping
{
   public class PaymentMap : EntityTypeConfiguration<Payment>
    {
       public PaymentMap()
       {
            //key
            HasKey(t => t.Id);
            //properties           
            Property(t => t.Amount).IsRequired();
            Property(t => t.Status).IsRequired();
            Property(t => t.CreateDate).IsRequired();
            Property(t => t.Content).HasMaxLength(200).HasColumnType("nvarchar");
           Property(t => t.UserId).HasMaxLength(128).HasColumnType("nvarchar");
           //table
           ToTable("Payment");
       }       
    }
}
