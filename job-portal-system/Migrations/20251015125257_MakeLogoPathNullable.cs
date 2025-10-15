using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace job_portal_system.Migrations
{
    /// <inheritdoc />
    public partial class MakeLogoPathNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "e5badfce-c94d-4fd5-b3b2-3148dfe9ba6c");

            migrationBuilder.AlterColumn<string>(
                name: "LogoPath",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LogoPath",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                schema: "Security",
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e5badfce-c94d-4fd5-b3b2-3148dfe9ba6c", 0, "4c8b4ead-5fad-4b12-b725-771ae419f02c", "user1@gmail.com", false, true, false, null, "USER1@GMAIL.COM", "USER1", "AQAAAAIAAYagAAAAEFiDL5hdVllhUvftPxAG0eORhdbKIBtX5gliAmtuyuNOnTfT94n0lcnwhv6bWHVt1A==", null, false, "ca5cb7a1-7dcd-40b3-a306-c3807865adaa", false, "User1" });
        }
    }
}
