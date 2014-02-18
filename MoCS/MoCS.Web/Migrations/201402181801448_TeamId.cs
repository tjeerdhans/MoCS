namespace MoCS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeamId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TeamId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TeamId");
        }
    }
}
