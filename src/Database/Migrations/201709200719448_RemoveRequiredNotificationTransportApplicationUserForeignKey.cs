namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRequiredNotificationTransportApplicationUserForeignKey : DbMigration
    {
        public override void Up()
        {
	        DropForeignKey("dbo.NotificationTransports", "UserId", "dbo.AspNetUsers");
	        /* Manually set ON DELETE SET NULL because EF can'not do it itself */
	        Sql("ALTER TABLE dbo.NotificationTransports ADD CONSTRAINT [FK_dbo.NotificationTransports_dbo.AspNetUsers_UserId] FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON UPDATE NO ACTION ON DELETE SET NULL");
		}
        
        public override void Down()
        {
	        DropForeignKey("dbo.NotificationTransports", "UserId", "dbo.AspNetUsers");
	        AddForeignKey("dbo.NotificationTransports", "UserId", "dbo.AspNetUsers", "Id");
		}
    }
}
