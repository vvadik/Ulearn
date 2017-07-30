using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddCertificates : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.Certificates",
					c => new
					{
						Id = c.Guid(nullable: false),
						TemplateId = c.Guid(nullable: false),
						UserId = c.String(nullable: false, maxLength: 128),
						InstructorId = c.String(nullable: false, maxLength: 128),
						Parameters = c.String(nullable: false),
						Timestamp = c.DateTime(nullable: false),
						IsDeleted = c.Boolean(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.AspNetUsers", t => t.InstructorId)
				.ForeignKey("dbo.CertificateTemplates", t => t.TemplateId, cascadeDelete: true)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId)
				.Index(t => t.TemplateId, name: "IDX_Certificate_ByTemplate")
				.Index(t => t.UserId, name: "IDX_Certificate_ByUser")
				.Index(t => t.InstructorId);

			CreateTable(
					"dbo.CertificateTemplates",
					c => new
					{
						Id = c.Guid(nullable: false),
						CourseId = c.String(nullable: false, maxLength: 40),
						Name = c.String(nullable: false),
						Timestamp = c.DateTime(nullable: false),
						IsDeleted = c.Boolean(nullable: false),
						ArchiveName = c.String(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.Index(t => t.CourseId, name: "IDX_CertificateTemplate_ByCourse");
		}

		public override void Down()
		{
			DropForeignKey("dbo.Certificates", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.Certificates", "TemplateId", "dbo.CertificateTemplates");
			DropForeignKey("dbo.Certificates", "InstructorId", "dbo.AspNetUsers");
			DropIndex("dbo.CertificateTemplates", "IDX_CertificateTemplate_ByCourse");
			DropIndex("dbo.Certificates", new[] { "InstructorId" });
			DropIndex("dbo.Certificates", "IDX_Certificate_ByUser");
			DropIndex("dbo.Certificates", "IDX_Certificate_ByTemplate");
			DropTable("dbo.CertificateTemplates");
			DropTable("dbo.Certificates");
		}
	}
}