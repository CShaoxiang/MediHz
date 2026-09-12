using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediHz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPrePackagedLabelGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "LabelGroup",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.InsertData(
                table: "LabelGroup",
                columns: new[] { "Id", "CreatedAtUtc", "Name", "OwnerId", "UpdatedAtUtc" },
                values: new object[,]
                {
                    {
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        "CirCor",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        "HF Lung V1",
                        null,
                        null,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "Label",
                columns: new[]
                {
                    "Id",
                    "CreatedAtUtc",
                    "LabelGroupId",
                    "Name",
                    "OwnerId",
                    "UpdatedAtUtc",
                },
                values: new object[,]
                {
                    {
                        new Guid("00000000-0000-0000-0000-000000000101"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.s1",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000102"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.systole",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000103"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.s2",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000104"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.diastole",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000105"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.unknown",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000106"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.murmur.systolic",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000107"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "cardiac.murmur.diastolic",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000201"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "resp.phase.inhalation",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000202"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "resp.phase.exhalation",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000203"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "resp.cas.wheeze",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000204"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "resp.cas.stridor",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000205"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "resp.cas.rhonchi",
                        null,
                        null,
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000206"),
                        new DateTimeOffset(
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            new TimeSpan(0, 0, 0, 0, 0)
                        ),
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "resp.das.crackle",
                        null,
                        null,
                    },
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000101")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000102")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000103")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000104")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000105")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000106")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000107")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000201")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000202")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000203")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000204")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000205")
            );

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000206")
            );

            migrationBuilder.DeleteData(
                table: "LabelGroup",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001")
            );

            migrationBuilder.DeleteData(
                table: "LabelGroup",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002")
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "LabelGroup",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true
            );
        }
    }
}
