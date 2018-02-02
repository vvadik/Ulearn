using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Database.Migrations
{
	/// <summary>
	/// MS SQL Express edition doesn't support full text catalogs and indexes. If you have Express edition, you should ignore this migration or install Full edition.
	/// </summary>
	public partial class AddFullTextIndexes : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			/* Full-text index for AspNetUsers.Names */
			migrationBuilder.Sql("CREATE FULLTEXT CATALOG AspNetUsersFullTextCatalog WITH ACCENT_SENSITIVITY = OFF", true);
			migrationBuilder.Sql("CREATE FULLTEXT INDEX ON AspNetUsers ([Names] LANGUAGE [Russian]) KEY INDEX [PK_AspNetUsers] ON (AspNetUsersFullTextCatalog, FILEGROUP[PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)", true);
			
			/* Full-text index for ExerciseSolutionByGraders.ClientUserId */
			migrationBuilder.Sql("CREATE FULLTEXT CATALOG ExerciseSolutionByGradersFullTextCatalog WITH ACCENT_SENSITIVITY = OFF", true);
			migrationBuilder.Sql("CREATE FULLTEXT INDEX ON ExerciseSolutionByGraders ([ClientUserId] LANGUAGE [Russian]) KEY INDEX [PK_ExerciseSolutionByGraders] ON (ExerciseSolutionByGradersFullTextCatalog, FILEGROUP[PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)", true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP FULLTEXT INDEX ON AspNetUsers", true);
			migrationBuilder.Sql("DROP FULLTEXT CATALOG AspNetUsersFullTextCatalog", true);
			
			migrationBuilder.Sql("DROP FULLTEXT INDEX ON ExerciseSolutionByGraders", true);
			migrationBuilder.Sql("DROP FULLTEXT CATALOG ExerciseSolutionByGradersFullTextCatalog", true);
		}
	}
}
