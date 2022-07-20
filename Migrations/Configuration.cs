using System.Data.Entity.Migrations;
using System.Linq;

namespace Documents.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Models.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Models.DataContext context)
        {
            Models.Entities.Position adminPos = context.Positions
                .FirstOrDefault(p => p.Name.ToLower() == "admin" || p.Name.ToLower() == "администратор");
            if (adminPos == null)
            {
                adminPos = context.Positions.Add(new Models.Entities.Position() { Name = "Администратор" });
                context.SaveChanges();
            }

            if (context.Users.Count() == 0)
            {
                var admin = Models.Entities.User.CreateAdmin();
                admin.PositionId = adminPos.Id;
                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
