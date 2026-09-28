using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediHz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SupportMultipleLabelsPerAnnotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annotations_Label_LabelId",
                table: "Annotations"
            );

            migrationBuilder.DropIndex(name: "IX_Annotations_LabelId", table: "Annotations");

            migrationBuilder.DropColumn(name: "LabelId", table: "Annotations");

            migrationBuilder.CreateTable(
                name: "AnnotationLabels",
                columns: table => new
                {
                    AnnotationsId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelsId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_AnnotationLabels",
                        x => new { x.AnnotationsId, x.LabelsId }
                    );
                    table.ForeignKey(
                        name: "FK_AnnotationLabels_Annotations_AnnotationsId",
                        column: x => x.AnnotationsId,
                        principalTable: "Annotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_AnnotationLabels_Label_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationLabels_LabelsId",
                table: "AnnotationLabels",
                column: "LabelsId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AnnotationLabels");

            migrationBuilder.AddColumn<Guid>(
                name: "LabelId",
                table: "Annotations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_LabelId",
                table: "Annotations",
                column: "LabelId"
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
    }
}
