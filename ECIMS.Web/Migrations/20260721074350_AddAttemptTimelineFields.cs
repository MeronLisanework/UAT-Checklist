using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECIMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAttemptTimelineFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecidedById",
                table: "ProjectUatAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecidedDate",
                table: "ProjectUatAttempts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "ProjectUatAttempts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUatAttempts_DecidedById",
                table: "ProjectUatAttempts",
                column: "DecidedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUatAttempts_Users_DecidedById",
                table: "ProjectUatAttempts",
                column: "DecidedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUatAttempts_Users_DecidedById",
                table: "ProjectUatAttempts");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUatAttempts_DecidedById",
                table: "ProjectUatAttempts");

            migrationBuilder.DropColumn(
                name: "DecidedById",
                table: "ProjectUatAttempts");

            migrationBuilder.DropColumn(
                name: "DecidedDate",
                table: "ProjectUatAttempts");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "ProjectUatAttempts");
        }
    }
}
