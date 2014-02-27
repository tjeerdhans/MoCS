using Microsoft.AspNet.Identity.EntityFramework;
using MoCS.Web.Models;
using System.Data.Entity.Migrations;

namespace MoCS.Web.Migrations
{


    internal sealed class Configuration : DbMigrationsConfiguration<MoCS.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MoCS.Web.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            var adminRole = new IdentityRole { Name = "Admin" };
            context.Roles.AddOrUpdate(r => r.Name, adminRole);
            var newUser = new ApplicationUser
            {
                UserName = "tjeerdhans",
                PasswordHash = "AKfDzhCAXv8mcW++4ZLy3uw11hPg5m1BwnhFnFJAs2TK/qh+qNiLrOu7AQWKRh3NeQ==",
                TeamId = null
            };
            var adminUserRole = new IdentityUserRole { User = newUser, Role = adminRole };

            newUser.Roles.Add(adminUserRole);
            context.Users.AddOrUpdate(u => u.UserName, newUser);
        }
    }
}
