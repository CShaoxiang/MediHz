using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediHz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAudioAndEntityBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedAtUtc"
            );

            migrationBuilder.RenameColumn(name: "UserId", table: "Users", newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Projects",
                newName: "CreatedAtUtc"
            );

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Annotations",
                newName: "UpdatedAtUtc"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Annotations",
                newName: "CreatedAtUtc"
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAtUtc",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAtUtc",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "Channels",
                table: "AudioFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "AudioFiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(
                    new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new TimeSpan(0, 0, 0, 0, 0)
                )
            );

            migrationBuilder.AddColumn<decimal>(
                name: "SampleRate",
                table: "AudioFiles",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAtUtc",
                table: "AudioFiles",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_AudioFiles_Channels_Positive",
                table: "AudioFiles",
                sql: "\"Channels\" > 0"
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_AudioFiles_SamplingRate_Positive",
                table: "AudioFiles",
                sql: "\"SampleRate\" > 0"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_AudioFiles_Channels_Positive",
                table: "AudioFiles"
            );

            migrationBuilder.DropCheckConstraint(
                name: "CK_AudioFiles_SamplingRate_Positive",
                table: "AudioFiles"
            );

            migrationBuilder.DropColumn(name: "UpdatedAtUtc", table: "Users");

            migrationBuilder.DropColumn(name: "UpdatedAtUtc", table: "Projects");

            migrationBuilder.DropColumn(name: "Channels", table: "AudioFiles");

            migrationBuilder.DropColumn(name: "CreatedAtUtc", table: "AudioFiles");

            migrationBuilder.DropColumn(name: "SampleRate", table: "AudioFiles");

            migrationBuilder.DropColumn(name: "UpdatedAtUtc", table: "AudioFiles");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Users",
                newName: "CreatedAt"
            );

            migrationBuilder.RenameColumn(name: "Id", table: "Users", newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Projects",
                newName: "CreatedAt"
            );

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "Annotations",
                newName: "UpdatedAt"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Annotations",
                newName: "CreatedAt"
            );
        }
    }
}
