using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediHz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LabelsAndLabelGroupsAndNavigationFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId1",
                table: "ProjectMembers"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Users_UserId1",
                table: "ProjectMembers"
            );

            migrationBuilder.DropIndex(
                name: "IX_ProjectMembers_ProjectId1",
                table: "ProjectMembers"
            );

            migrationBuilder.DropIndex(name: "IX_ProjectMembers_UserId1", table: "ProjectMembers");

            migrationBuilder.DropColumn(name: "ProjectId1", table: "ProjectMembers");

            migrationBuilder.DropColumn(name: "UserId1", table: "ProjectMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "LabelId",
                table: "Annotations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateTable(
                name: "LabelGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    CreatedAtUtc = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabelGroup_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    CreatedAtUtc = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Label_LabelGroup_LabelGroupId",
                        column: x => x.LabelGroupId,
                        principalTable: "LabelGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_Label_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_LabelId",
                table: "Annotations",
                column: "LabelId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Label_LabelGroupId",
                table: "Label",
                column: "LabelGroupId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Label_OwnerId",
                table: "Label",
                column: "OwnerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LabelGroup_OwnerId",
                table: "LabelGroup",
                column: "OwnerId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Annotations_Label_LabelId",
                table: "Annotations",
                column: "LabelId",
                principalTable: "Label",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annotations_Label_LabelId",
                table: "Annotations"
            );

            migrationBuilder.DropTable(name: "Label");

            migrationBuilder.DropTable(name: "LabelGroup");

            migrationBuilder.DropIndex(name: "IX_Annotations_LabelId", table: "Annotations");

            migrationBuilder.DropColumn(name: "LabelId", table: "Annotations");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId1",
                table: "ProjectMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "ProjectMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId1",
                table: "ProjectMembers",
                column: "ProjectId1"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId1",
                table: "ProjectMembers",
                column: "UserId1"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId1",
                table: "ProjectMembers",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Users_UserId1",
                table: "ProjectMembers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
