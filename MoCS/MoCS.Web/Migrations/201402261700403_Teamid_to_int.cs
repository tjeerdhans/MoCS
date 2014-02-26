namespace MoCS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Teamid_to_int : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "TeamId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "TeamId", c => c.Guid());
        }
    }
}
