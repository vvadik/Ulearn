using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Database.Migrations
{
    public partial class AddCommonFeedNotificationTransportAndReplaceAdminRole : Migration
    {
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("INSERT INTO dbo.NotificationTransports (UserId, IsEnabled, IsDeleted, Discriminator) VALUES (NULL, 1, 0, 'FeedNotificationTransport')");
			migrationBuilder.Sql("UPDATE AspNetRoles SET Name = N'SysAdmin' WHERE Name = N'admin'");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("UPDATE AspNetRoles SET Name = N'admin' WHERE Name = N'SysAdmin'");
			migrationBuilder.Sql("DELETE FROM dbo.NotificationTransports WHERE Discriminator = 'FeedNotificationTransport' AND UserId IS NULL");
		}

    }
}
