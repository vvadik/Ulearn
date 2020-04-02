namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class AddIndexToNotificationDeliveryByTransportAndCreateTimeIncludeNotificationId : DbMigration
    {
		private string indexName = "IDX_NotificationDelivery_ByTransportAndCreateTime_IncludeNotificationId";

		public override void Up()
		{
			Sql($"CREATE NONCLUSTERED INDEX [{indexName}] ON [dbo].[NotificationDeliveries] ([NotificationTransportId], [CreateTime]) INCLUDE ([NotificationId])");
		}
		
		public override void Down()
		{
			Sql($"DROP INDEX [{indexName}]");
		}
    }
}
