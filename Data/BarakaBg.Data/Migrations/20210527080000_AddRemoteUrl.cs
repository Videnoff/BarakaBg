namespace BarakaBg.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddRemoteUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemoteImageUrl",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoteImageUrl",
                table: "Images");
        }
    }
}
