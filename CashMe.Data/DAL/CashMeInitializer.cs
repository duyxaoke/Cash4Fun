using System.Data.Entity.Migrations;

namespace CashMe.Data.DAL
{
    public class CashMeInitializer : DbMigrationsConfiguration<CashMeContext>
    {
        public CashMeInitializer()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
        protected override void Seed(CashMeContext context)
        {

            base.Seed(context);
        }
    }
}
