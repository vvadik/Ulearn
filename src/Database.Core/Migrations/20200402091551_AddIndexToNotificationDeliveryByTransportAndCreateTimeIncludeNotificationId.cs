using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddIndexToNotificationDeliveryByTransportAndCreateTimeIncludeNotificationId : Migration
    {
		private string indexName = "IDX_NotificationDelivery_ByTransportAndCreateTime_IncludeNotificationId";
		
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"CREATE NONCLUSTERED INDEX [{indexName}] ON [dbo].[NotificationDeliveries] ([NotificationTransportId], [CreateTime]) INCLUDE ([NotificationId])");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"DROP INDEX [{indexName}]");
		}
    }
}
