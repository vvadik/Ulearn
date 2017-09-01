namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStepikExportProcessAndMapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StepikExportProcesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UlearnCourseId = c.String(nullable: false, maxLength: 100),
                        StepikCourseId = c.Int(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        StartTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId, name: "IDX_StepikExportProcess_ByOwner");
            
            CreateTable(
                "dbo.StepikExportSlideAndStepMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UlearnCourseId = c.String(nullable: false, maxLength: 100),
                        StepikCourseId = c.Int(nullable: false),
                        SlideId = c.Guid(nullable: false),
                        StepId = c.Int(nullable: false),
                        SlideXml = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UlearnCourseId, name: "IDX_StepikExportSlideAndStepMap_ByUlearnCourseId")
                .Index(t => new { t.UlearnCourseId, t.SlideId }, name: "IDX_StepikExportSlideAndStepMap_ByUlearnCourseIdAndSlideId")
                .Index(t => new { t.UlearnCourseId, t.StepikCourseId }, name: "IDX_StepikExportSlideAndStepMap_ByUlearnCourseIdAndStepikCourseId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StepikExportProcesses", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.StepikExportSlideAndStepMaps", "IDX_StepikExportSlideAndStepMap_ByUlearnCourseIdAndStepikCourseId");
            DropIndex("dbo.StepikExportSlideAndStepMaps", "IDX_StepikExportSlideAndStepMap_ByUlearnCourseIdAndSlideId");
            DropIndex("dbo.StepikExportSlideAndStepMaps", "IDX_StepikExportSlideAndStepMap_ByUlearnCourseId");
            DropIndex("dbo.StepikExportProcesses", "IDX_StepikExportProcess_ByOwner");
            DropTable("dbo.StepikExportSlideAndStepMaps");
            DropTable("dbo.StepikExportProcesses");
        }
    }
}
