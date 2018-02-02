using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    AvatarUrl = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    Gender = table.Column<short>(nullable: true),
                    KonturLogin = table.Column<string>(maxLength: 200, nullable: true),
                    LastConfirmationEmailTime = table.Column<DateTime>(nullable: true),
                    LastEdit = table.Column<DateTime>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    Names = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    Registered = table.Column<DateTime>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TelegramChatId = table.Column<long>(nullable: true),
                    TelegramChatTitle = table.Column<string>(maxLength: 200, nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchiveName = table.Column<string>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 40, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentsPolicies",
                columns: table => new
                {
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    IsCommentsEnabled = table.Column<bool>(nullable: false),
                    ModerationPolicy = table.Column<int>(nullable: false),
                    OnlyInstructorsCanReply = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsPolicies", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "Consumers",
                columns: table => new
                {
                    ConsumerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Secret = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumers", x => x.ConsumerId);
                });

            migrationBuilder.CreateTable(
                name: "LtiRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Request = table.Column<string>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LtiRequests", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "QuizVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    LoadingTime = table.Column<DateTime>(nullable: false),
                    NormalizedXml = table.Column<string>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StepikExportSlideAndStepMaps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SlideId = table.Column<Guid>(nullable: false),
                    SlideXml = table.Column<string>(nullable: false),
                    StepId = table.Column<int>(nullable: false),
                    StepikCourseId = table.Column<int>(nullable: false),
                    UlearnCourseId = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepikExportSlideAndStepMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Texts",
                columns: table => new
                {
                    Hash = table.Column<string>(maxLength: 40, nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Texts", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "UnitAppearances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    PublishTime = table.Column<DateTime>(nullable: false),
                    UnitId = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitAppearances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    InstructorId = table.Column<string>(maxLength: 64, nullable: false),
                    Score = table.Column<int>(nullable: false),
                    ScoringGroupId = table.Column<string>(maxLength: 64, nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UnitId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalScores_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdditionalScores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomaticQuizCheckings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    Score = table.Column<int>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomaticQuizCheckings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomaticQuizCheckings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(maxLength: 64, nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    IsCorrectAnswer = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsPinnedToTop = table.Column<bool>(nullable: false),
                    ParentCommentId = table.Column<int>(nullable: false),
                    PublishTime = table.Column<DateTime>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessType = table.Column<short>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    GrantTime = table.Column<DateTime>(nullable: false),
                    GrantedById = table.Column<string>(maxLength: 64, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseAccesses_AspNetUsers_GrantedById",
                        column: x => x.GrantedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<string>(maxLength: 64, nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    LoadingTime = table.Column<DateTime>(nullable: false),
                    PublishTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseVersions_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraderClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraderClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraderClients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupLabels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColorHex = table.Column<string>(maxLength: 6, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    OwnerId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupLabels_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CanUsersSeeGroupProgress = table.Column<bool>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    DefaultProhibitFutherReview = table.Column<bool>(nullable: false),
                    InviteHash = table.Column<Guid>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsInviteLinkEnabled = table.Column<bool>(nullable: false),
                    IsManualCheckingEnabled = table.Column<bool>(nullable: false),
                    IsManualCheckingEnabledForOldSolutions = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 300, nullable: false),
                    OwnerId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    HintId = table.Column<int>(nullable: false),
                    IsHintHelped = table.Column<bool>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hints_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManualQuizCheckings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    IsChecked = table.Column<bool>(nullable: false),
                    LockedById = table.Column<string>(maxLength: 64, nullable: true),
                    LockedUntil = table.Column<DateTime>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualQuizCheckings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualQuizCheckings_AspNetUsers_LockedById",
                        column: x => x.LockedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualQuizCheckings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTransports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTransports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTransports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestoreRequests",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestoreRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestoreRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlideRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlideRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlideRates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepikAccessTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(maxLength: 100, nullable: false),
                    AddedTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepikAccessTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepikAccessTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepikExportProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    IsFinished = table.Column<bool>(nullable: false),
                    IsInitialExport = table.Column<bool>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    Log = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(maxLength: 64, nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    StepikCourseId = table.Column<int>(nullable: false),
                    StepikCourseTitle = table.Column<string>(maxLength: 100, nullable: true),
                    UlearnCourseId = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepikExportProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepikExportProcesses_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessType = table.Column<short>(nullable: false),
                    GrantTime = table.Column<DateTime>(nullable: false),
                    GrantedById = table.Column<string>(maxLength: 64, nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAccesses_AspNetUsers_GrantedById",
                        column: x => x.GrantedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserQuestions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: true),
                    Question = table.Column<string>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    SlideTitle = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    UnitName = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false),
                    UserName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuestions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_UserQuestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttemptsCount = table.Column<int>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    HasManualChecking = table.Column<bool>(nullable: false),
                    IsPassed = table.Column<bool>(nullable: false),
                    IsSkipped = table.Column<bool>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XQueueWatchers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BaseUrl = table.Column<string>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    QueueName = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XQueueWatchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XQueueWatchers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InstructorId = table.Column<string>(maxLength: 64, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsPreview = table.Column<bool>(nullable: false),
                    Parameters = table.Column<string>(nullable: false),
                    TemplateId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_CertificateTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "CertificateTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserQuizzes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    IsRightAnswer = table.Column<bool>(nullable: false),
                    ItemId = table.Column<string>(maxLength: 64, nullable: true),
                    QuizBlockMaxScore = table.Column<int>(nullable: false),
                    QuizBlockScore = table.Column<int>(nullable: false),
                    QuizId = table.Column<string>(maxLength: 64, nullable: true),
                    QuizVersionId = table.Column<int>(nullable: true),
                    SlideId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 1024, nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false),
                    isDropped = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQuizzes_QuizVersions_QuizVersionId",
                        column: x => x.QuizVersionId,
                        principalTable: "QuizVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserQuizzes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomaticExerciseCheckings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompilationErrorHash = table.Column<string>(maxLength: 40, nullable: true),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Elapsed = table.Column<TimeSpan>(nullable: true),
                    ExecutionServiceName = table.Column<string>(maxLength: 40, nullable: true),
                    IsCompilationError = table.Column<bool>(nullable: false),
                    IsRightAnswer = table.Column<bool>(nullable: false),
                    OutputHash = table.Column<string>(maxLength: 40, nullable: true),
                    Score = table.Column<int>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomaticExerciseCheckings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomaticExerciseCheckings_Texts_CompilationErrorHash",
                        column: x => x.CompilationErrorHash,
                        principalTable: "Texts",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomaticExerciseCheckings_Texts_OutputHash",
                        column: x => x.OutputHash,
                        principalTable: "Texts",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomaticExerciseCheckings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentLikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnabledAdditionalScoringGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    ScoringGroupId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnabledAdditionalScoringGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnabledAdditionalScoringGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessType = table.Column<short>(nullable: false),
                    GrantTime = table.Column<DateTime>(nullable: false),
                    GrantedById = table.Column<string>(maxLength: 64, nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAccesses_AspNetUsers_GrantedById",
                        column: x => x.GrantedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupAccesses_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddingTime = table.Column<DateTime>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabelsOnGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    LabelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelsOnGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabelsOnGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabelsOnGroups_GroupLabels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "GroupLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeedViewTimestamps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    TransportId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedViewTimestamps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedViewTimestamps_NotificationTransports_TransportId",
                        column: x => x.TransportId,
                        principalTable: "NotificationTransports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTransportSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 100, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    NotificationTransportId = table.Column<int>(nullable: false),
                    NotificationType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTransportSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTransportSettings_NotificationTransports_NotificationTransportId",
                        column: x => x.NotificationTransportId,
                        principalTable: "NotificationTransports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExerciseSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    AutomaticCheckingId = table.Column<int>(nullable: false),
                    AutomaticCheckingIsRightAnswer = table.Column<bool>(nullable: false),
                    CodeHash = table.Column<int>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 100, nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    SolutionCodeHash = table.Column<string>(maxLength: 40, nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExerciseSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExerciseSubmissions_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserExerciseSubmissions_AutomaticExerciseCheckings_AutomaticCheckingId",
                        column: x => x.AutomaticCheckingId,
                        principalTable: "AutomaticExerciseCheckings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExerciseSubmissions_Texts_SolutionCodeHash",
                        column: x => x.SolutionCodeHash,
                        principalTable: "Texts",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExerciseSubmissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSolutionByGraders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<Guid>(nullable: false),
                    ClientUserId = table.Column<string>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSolutionByGraders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSolutionByGraders_GraderClients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "GraderClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseSolutionByGraders_UserExerciseSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "UserExerciseSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManualExerciseCheckings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    IsChecked = table.Column<bool>(nullable: false),
                    LockedById = table.Column<string>(maxLength: 64, nullable: true),
                    LockedUntil = table.Column<DateTime>(nullable: true),
                    ProhibitFurtherManualCheckings = table.Column<bool>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualExerciseCheckings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualExerciseCheckings_AspNetUsers_LockedById",
                        column: x => x.LockedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualExerciseCheckings_UserExerciseSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "UserExerciseSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManualExerciseCheckings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolutionLikes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubmissionId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionLikes_UserExerciseSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "UserExerciseSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolutionLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XQueueExerciseSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsResultSent = table.Column<bool>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: false),
                    WatcherId = table.Column<int>(nullable: false),
                    XQueueHeader = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XQueueExerciseSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XQueueExerciseSubmissions_UserExerciseSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "UserExerciseSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XQueueExerciseSubmissions_XQueueWatchers_WatcherId",
                        column: x => x.WatcherId,
                        principalTable: "XQueueWatchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseCodeReviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(maxLength: 64, nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    ExerciseCheckingId = table.Column<int>(nullable: false),
                    FinishLine = table.Column<int>(nullable: false),
                    FinishPosition = table.Column<int>(nullable: false),
                    HiddenFromTopComments = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    StartLine = table.Column<int>(nullable: false),
                    StartPosition = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseCodeReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseCodeReviews_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExerciseCodeReviews_ManualExerciseCheckings_ExerciseCheckingId",
                        column: x => x.ExerciseCheckingId,
                        principalTable: "ManualExerciseCheckings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    AddedUserId = table.Column<string>(maxLength: 64, nullable: true),
                    ProcessId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true),
                    AccessId = table.Column<int>(nullable: true),
                    GroupMemberHasBeenRemovedNotification_GroupId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    JoinedToYourGroupNotification_GroupId = table.Column<int>(nullable: true),
                    JoinedUserId = table.Column<string>(maxLength: 64, nullable: true),
                    CommentId = table.Column<int>(nullable: true),
                    LikedUserId = table.Column<string>(maxLength: 64, nullable: true),
                    NewCommentNotification_CommentId = table.Column<int>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreDeliveriesCreated = table.Column<bool>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 100, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    InitiatedById = table.Column<string>(maxLength: 64, nullable: false),
                    CheckingId = table.Column<int>(nullable: true),
                    PassedManualQuizCheckingNotification_CheckingId = table.Column<int>(nullable: true),
                    CourseVersionId = table.Column<Guid>(nullable: true),
                    ScoreId = table.Column<int>(nullable: true),
                    RepliedToYourCommentNotification_CommentId = table.Column<int>(nullable: true),
                    ParentCommentId = table.Column<int>(nullable: true),
                    RevokedAccessToGroupNotification_AccessId = table.Column<int>(nullable: true),
                    UploadedPackageNotification_CourseVersionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_AddedUserId",
                        column: x => x.AddedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_StepikExportProcesses_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "StepikExportProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_GroupAccesses_AccessId",
                        column: x => x.AccessId,
                        principalTable: "GroupAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Groups_GroupMemberHasBeenRemovedNotification_GroupId",
                        column: x => x.GroupMemberHasBeenRemovedNotification_GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Groups_JoinedToYourGroupNotification_GroupId",
                        column: x => x.JoinedToYourGroupNotification_GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_JoinedUserId",
                        column: x => x.JoinedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_LikedUserId",
                        column: x => x.LikedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Comments_NewCommentNotification_CommentId",
                        column: x => x.NewCommentNotification_CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_InitiatedById",
                        column: x => x.InitiatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_ManualExerciseCheckings_CheckingId",
                        column: x => x.CheckingId,
                        principalTable: "ManualExerciseCheckings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_ManualQuizCheckings_PassedManualQuizCheckingNotification_CheckingId",
                        column: x => x.PassedManualQuizCheckingNotification_CheckingId,
                        principalTable: "ManualQuizCheckings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_CourseVersions_CourseVersionId",
                        column: x => x.CourseVersionId,
                        principalTable: "CourseVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AdditionalScores_ScoreId",
                        column: x => x.ScoreId,
                        principalTable: "AdditionalScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Comments_RepliedToYourCommentNotification_CommentId",
                        column: x => x.RepliedToYourCommentNotification_CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_GroupAccesses_RevokedAccessToGroupNotification_AccessId",
                        column: x => x.RevokedAccessToGroupNotification_AccessId,
                        principalTable: "GroupAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_CourseVersions_UploadedPackageNotification_CourseVersionId",
                        column: x => x.UploadedPackageNotification_CourseVersionId,
                        principalTable: "CourseVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    FailsCount = table.Column<int>(nullable: false),
                    NextTryTime = table.Column<DateTime>(nullable: true),
                    NotificationId = table.Column<int>(nullable: false),
                    NotificationTransportId = table.Column<int>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_NotificationTransports_NotificationTransportId",
                        column: x => x.NotificationTransportId,
                        principalTable: "NotificationTransports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalScores_InstructorId",
                table: "AdditionalScores",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalScores_UnitId",
                table: "AdditionalScores",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalScores_UserId",
                table: "AdditionalScores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalScores_CourseId_UserId",
                table: "AdditionalScores",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalScores_CourseId_UserId_UnitId_ScoringGroupId",
                table: "AdditionalScores",
                columns: new[] { "CourseId", "UserId", "UnitId", "ScoringGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TelegramChatId",
                table: "AspNetUsers",
                column: "TelegramChatId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_CompilationErrorHash",
                table: "AutomaticExerciseCheckings",
                column: "CompilationErrorHash");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_IsRightAnswer",
                table: "AutomaticExerciseCheckings",
                column: "IsRightAnswer");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_OutputHash",
                table: "AutomaticExerciseCheckings",
                column: "OutputHash");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_UserId",
                table: "AutomaticExerciseCheckings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_CourseId_SlideId",
                table: "AutomaticExerciseCheckings",
                columns: new[] { "CourseId", "SlideId" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_CourseId_UserId",
                table: "AutomaticExerciseCheckings",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_CourseId_SlideId_Timestamp",
                table: "AutomaticExerciseCheckings",
                columns: new[] { "CourseId", "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_CourseId_SlideId_UserId",
                table: "AutomaticExerciseCheckings",
                columns: new[] { "CourseId", "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticQuizCheckings_UserId",
                table: "AutomaticQuizCheckings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticQuizCheckings_CourseId_SlideId",
                table: "AutomaticQuizCheckings",
                columns: new[] { "CourseId", "SlideId" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticQuizCheckings_CourseId_UserId",
                table: "AutomaticQuizCheckings",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticQuizCheckings_CourseId_SlideId_Timestamp",
                table: "AutomaticQuizCheckings",
                columns: new[] { "CourseId", "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticQuizCheckings_CourseId_SlideId_UserId",
                table: "AutomaticQuizCheckings",
                columns: new[] { "CourseId", "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_InstructorId",
                table: "Certificates",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_TemplateId",
                table: "Certificates",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTemplates_CourseId",
                table: "CertificateTemplates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_CommentId",
                table: "CommentLikes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_UserId_CommentId",
                table: "CommentLikes",
                columns: new[] { "UserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_SlideId",
                table: "Comments",
                column: "SlideId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId_PublishTime",
                table: "Comments",
                columns: new[] { "AuthorId", "PublishTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Consumers_Key",
                table: "Consumers",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAccesses_CourseId",
                table: "CourseAccesses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAccesses_GrantTime",
                table: "CourseAccesses",
                column: "GrantTime");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAccesses_GrantedById",
                table: "CourseAccesses",
                column: "GrantedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAccesses_UserId",
                table: "CourseAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAccesses_CourseId_IsEnabled",
                table: "CourseAccesses",
                columns: new[] { "CourseId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAccesses_CourseId_UserId_IsEnabled",
                table: "CourseAccesses",
                columns: new[] { "CourseId", "UserId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseVersions_AuthorId",
                table: "CourseVersions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseVersions_CourseId_LoadingTime",
                table: "CourseVersions",
                columns: new[] { "CourseId", "LoadingTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseVersions_CourseId_PublishTime",
                table: "CourseVersions",
                columns: new[] { "CourseId", "PublishTime" });

            migrationBuilder.CreateIndex(
                name: "IX_EnabledAdditionalScoringGroups_GroupId",
                table: "EnabledAdditionalScoringGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseCodeReviews_AuthorId",
                table: "ExerciseCodeReviews",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseCodeReviews_ExerciseCheckingId",
                table: "ExerciseCodeReviews",
                column: "ExerciseCheckingId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSolutionByGraders_ClientId",
                table: "ExerciseSolutionByGraders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSolutionByGraders_SubmissionId",
                table: "ExerciseSolutionByGraders",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedViewTimestamps_Timestamp",
                table: "FeedViewTimestamps",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FeedViewTimestamps_TransportId",
                table: "FeedViewTimestamps",
                column: "TransportId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedViewTimestamps_UserId",
                table: "FeedViewTimestamps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedViewTimestamps_UserId_TransportId",
                table: "FeedViewTimestamps",
                columns: new[] { "UserId", "TransportId" });

            migrationBuilder.CreateIndex(
                name: "IX_GraderClients_UserId",
                table: "GraderClients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAccesses_GrantTime",
                table: "GroupAccesses",
                column: "GrantTime");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAccesses_GrantedById",
                table: "GroupAccesses",
                column: "GrantedById");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAccesses_GroupId",
                table: "GroupAccesses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAccesses_UserId",
                table: "GroupAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAccesses_GroupId_IsEnabled",
                table: "GroupAccesses",
                columns: new[] { "GroupId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAccesses_GroupId_UserId_IsEnabled",
                table: "GroupAccesses",
                columns: new[] { "GroupId", "UserId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupLabels_OwnerId",
                table: "GroupLabels",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupLabels_OwnerId_IsDeleted",
                table: "GroupLabels",
                columns: new[] { "OwnerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_UserId",
                table: "GroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CourseId",
                table: "Groups",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_InviteHash",
                table: "Groups",
                column: "InviteHash");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Hints_UserId",
                table: "Hints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Hints_SlideId_HintId_UserId_IsHintHelped",
                table: "Hints",
                columns: new[] { "SlideId", "HintId", "UserId", "IsHintHelped" });

            migrationBuilder.CreateIndex(
                name: "IX_LabelsOnGroups_GroupId",
                table: "LabelsOnGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LabelsOnGroups_LabelId",
                table: "LabelsOnGroups",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_LabelsOnGroups_GroupId_LabelId",
                table: "LabelsOnGroups",
                columns: new[] { "GroupId", "LabelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LtiRequests_SlideId_UserId",
                table: "LtiRequests",
                columns: new[] { "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_LockedById",
                table: "ManualExerciseCheckings",
                column: "LockedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_SubmissionId",
                table: "ManualExerciseCheckings",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_UserId",
                table: "ManualExerciseCheckings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_CourseId_SlideId",
                table: "ManualExerciseCheckings",
                columns: new[] { "CourseId", "SlideId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_CourseId_UserId",
                table: "ManualExerciseCheckings",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_CourseId_SlideId_Timestamp",
                table: "ManualExerciseCheckings",
                columns: new[] { "CourseId", "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualExerciseCheckings_CourseId_SlideId_UserId_ProhibitFurtherManualCheckings",
                table: "ManualExerciseCheckings",
                columns: new[] { "CourseId", "SlideId", "UserId", "ProhibitFurtherManualCheckings" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualQuizCheckings_LockedById",
                table: "ManualQuizCheckings",
                column: "LockedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManualQuizCheckings_UserId",
                table: "ManualQuizCheckings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualQuizCheckings_CourseId_SlideId",
                table: "ManualQuizCheckings",
                columns: new[] { "CourseId", "SlideId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualQuizCheckings_CourseId_UserId",
                table: "ManualQuizCheckings",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualQuizCheckings_CourseId_SlideId_Timestamp",
                table: "ManualQuizCheckings",
                columns: new[] { "CourseId", "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualQuizCheckings_CourseId_SlideId_UserId",
                table: "ManualQuizCheckings",
                columns: new[] { "CourseId", "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_CreateTime",
                table: "NotificationDeliveries",
                column: "CreateTime");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NextTryTime",
                table: "NotificationDeliveries",
                column: "NextTryTime");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationTransportId",
                table: "NotificationDeliveries",
                column: "NotificationTransportId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationId_NotificationTransportId",
                table: "NotificationDeliveries",
                columns: new[] { "NotificationId", "NotificationTransportId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AddedUserId",
                table: "Notifications",
                column: "AddedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ProcessId",
                table: "Notifications",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_GroupId",
                table: "Notifications",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AccessId",
                table: "Notifications",
                column: "AccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_GroupMemberHasBeenRemovedNotification_GroupId",
                table: "Notifications",
                column: "GroupMemberHasBeenRemovedNotification_GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_JoinedToYourGroupNotification_GroupId",
                table: "Notifications",
                column: "JoinedToYourGroupNotification_GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_JoinedUserId",
                table: "Notifications",
                column: "JoinedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentId",
                table: "Notifications",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LikedUserId",
                table: "Notifications",
                column: "LikedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NewCommentNotification_CommentId",
                table: "Notifications",
                column: "NewCommentNotification_CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AreDeliveriesCreated",
                table: "Notifications",
                column: "AreDeliveriesCreated");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CourseId",
                table: "Notifications",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreateTime",
                table: "Notifications",
                column: "CreateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_InitiatedById",
                table: "Notifications",
                column: "InitiatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CheckingId",
                table: "Notifications",
                column: "CheckingId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PassedManualQuizCheckingNotification_CheckingId",
                table: "Notifications",
                column: "PassedManualQuizCheckingNotification_CheckingId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CourseVersionId",
                table: "Notifications",
                column: "CourseVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ScoreId",
                table: "Notifications",
                column: "ScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RepliedToYourCommentNotification_CommentId",
                table: "Notifications",
                column: "RepliedToYourCommentNotification_CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ParentCommentId",
                table: "Notifications",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RevokedAccessToGroupNotification_AccessId",
                table: "Notifications",
                column: "RevokedAccessToGroupNotification_AccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UploadedPackageNotification_CourseVersionId",
                table: "Notifications",
                column: "UploadedPackageNotification_CourseVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTransports_UserId",
                table: "NotificationTransports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTransports_UserId_IsDeleted",
                table: "NotificationTransports",
                columns: new[] { "UserId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTransportSettings_CourseId",
                table: "NotificationTransportSettings",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTransportSettings_NotificationTransportId",
                table: "NotificationTransportSettings",
                column: "NotificationTransportId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTransportSettings_NotificationType",
                table: "NotificationTransportSettings",
                column: "NotificationType");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTransportSettings_CourseId_NotificationType",
                table: "NotificationTransportSettings",
                columns: new[] { "CourseId", "NotificationType" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizVersions_SlideId",
                table: "QuizVersions",
                column: "SlideId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizVersions_SlideId_LoadingTime",
                table: "QuizVersions",
                columns: new[] { "SlideId", "LoadingTime" });

            migrationBuilder.CreateIndex(
                name: "IX_RestoreRequests_UserId",
                table: "RestoreRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SlideRates_UserId",
                table: "SlideRates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SlideRates_SlideId_UserId",
                table: "SlideRates",
                columns: new[] { "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_SolutionLikes_SubmissionId",
                table: "SolutionLikes",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionLikes_UserId_SubmissionId",
                table: "SolutionLikes",
                columns: new[] { "UserId", "SubmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StepikAccessTokens_AddedTime",
                table: "StepikAccessTokens",
                column: "AddedTime");

            migrationBuilder.CreateIndex(
                name: "IX_StepikAccessTokens_UserId",
                table: "StepikAccessTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StepikExportProcesses_OwnerId",
                table: "StepikExportProcesses",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StepikExportSlideAndStepMaps_UlearnCourseId",
                table: "StepikExportSlideAndStepMaps",
                column: "UlearnCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StepikExportSlideAndStepMaps_UlearnCourseId_SlideId",
                table: "StepikExportSlideAndStepMaps",
                columns: new[] { "UlearnCourseId", "SlideId" });

            migrationBuilder.CreateIndex(
                name: "IX_StepikExportSlideAndStepMaps_UlearnCourseId_StepikCourseId",
                table: "StepikExportSlideAndStepMaps",
                columns: new[] { "UlearnCourseId", "StepikCourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccesses_GrantTime",
                table: "SystemAccesses",
                column: "GrantTime");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccesses_GrantedById",
                table: "SystemAccesses",
                column: "GrantedById");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccesses_IsEnabled",
                table: "SystemAccesses",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccesses_UserId",
                table: "SystemAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccesses_UserId_IsEnabled",
                table: "SystemAccesses",
                columns: new[] { "UserId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_UnitAppearances_CourseId_PublishTime",
                table: "UnitAppearances",
                columns: new[] { "CourseId", "PublishTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_ApplicationUserId",
                table: "UserExerciseSubmissions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_AutomaticCheckingId",
                table: "UserExerciseSubmissions",
                column: "AutomaticCheckingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_AutomaticCheckingIsRightAnswer",
                table: "UserExerciseSubmissions",
                column: "AutomaticCheckingIsRightAnswer");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_SolutionCodeHash",
                table: "UserExerciseSubmissions",
                column: "SolutionCodeHash");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_UserId",
                table: "UserExerciseSubmissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_CourseId_AutomaticCheckingIsRightAnswer",
                table: "UserExerciseSubmissions",
                columns: new[] { "CourseId", "AutomaticCheckingIsRightAnswer" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_CourseId_SlideId",
                table: "UserExerciseSubmissions",
                columns: new[] { "CourseId", "SlideId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_CourseId_SlideId_AutomaticCheckingIsRightAnswer",
                table: "UserExerciseSubmissions",
                columns: new[] { "CourseId", "SlideId", "AutomaticCheckingIsRightAnswer" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_CourseId_SlideId_Timestamp",
                table: "UserExerciseSubmissions",
                columns: new[] { "CourseId", "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_CourseId_SlideId_UserId",
                table: "UserExerciseSubmissions",
                columns: new[] { "CourseId", "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestions_UserId",
                table: "UserQuestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_QuizVersionId",
                table: "UserQuizzes",
                column: "QuizVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_SlideId_Timestamp",
                table: "UserQuizzes",
                columns: new[] { "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_UserId_SlideId_isDropped_QuizId_ItemId",
                table: "UserQuizzes",
                columns: new[] { "UserId", "SlideId", "isDropped", "QuizId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_UserId",
                table: "Visits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_SlideId_Timestamp",
                table: "Visits",
                columns: new[] { "SlideId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_SlideId_UserId",
                table: "Visits",
                columns: new[] { "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_CourseId_SlideId_UserId",
                table: "Visits",
                columns: new[] { "CourseId", "SlideId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_XQueueExerciseSubmissions_SubmissionId",
                table: "XQueueExerciseSubmissions",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_XQueueExerciseSubmissions_WatcherId",
                table: "XQueueExerciseSubmissions",
                column: "WatcherId");

            migrationBuilder.CreateIndex(
                name: "IX_XQueueWatchers_UserId",
                table: "XQueueWatchers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AutomaticQuizCheckings");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropTable(
                name: "CommentsPolicies");

            migrationBuilder.DropTable(
                name: "Consumers");

            migrationBuilder.DropTable(
                name: "CourseAccesses");

            migrationBuilder.DropTable(
                name: "EnabledAdditionalScoringGroups");

            migrationBuilder.DropTable(
                name: "ExerciseCodeReviews");

            migrationBuilder.DropTable(
                name: "ExerciseSolutionByGraders");

            migrationBuilder.DropTable(
                name: "FeedViewTimestamps");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "Hints");

            migrationBuilder.DropTable(
                name: "LabelsOnGroups");

            migrationBuilder.DropTable(
                name: "LtiRequests");

            migrationBuilder.DropTable(
                name: "NotificationDeliveries");

            migrationBuilder.DropTable(
                name: "NotificationTransportSettings");

            migrationBuilder.DropTable(
                name: "RestoreRequests");

            migrationBuilder.DropTable(
                name: "SlideRates");

            migrationBuilder.DropTable(
                name: "SolutionLikes");

            migrationBuilder.DropTable(
                name: "StepikAccessTokens");

            migrationBuilder.DropTable(
                name: "StepikExportSlideAndStepMaps");

            migrationBuilder.DropTable(
                name: "SystemAccesses");

            migrationBuilder.DropTable(
                name: "UnitAppearances");

            migrationBuilder.DropTable(
                name: "UserQuestions");

            migrationBuilder.DropTable(
                name: "UserQuizzes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "XQueueExerciseSubmissions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CertificateTemplates");

            migrationBuilder.DropTable(
                name: "GraderClients");

            migrationBuilder.DropTable(
                name: "GroupLabels");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTransports");

            migrationBuilder.DropTable(
                name: "QuizVersions");

            migrationBuilder.DropTable(
                name: "XQueueWatchers");

            migrationBuilder.DropTable(
                name: "StepikExportProcesses");

            migrationBuilder.DropTable(
                name: "GroupAccesses");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ManualExerciseCheckings");

            migrationBuilder.DropTable(
                name: "ManualQuizCheckings");

            migrationBuilder.DropTable(
                name: "CourseVersions");

            migrationBuilder.DropTable(
                name: "AdditionalScores");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "UserExerciseSubmissions");

            migrationBuilder.DropTable(
                name: "AutomaticExerciseCheckings");

            migrationBuilder.DropTable(
                name: "Texts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
