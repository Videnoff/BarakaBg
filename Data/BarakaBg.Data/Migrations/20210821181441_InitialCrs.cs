namespace BarakaBg.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class InitialCrs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "ProductComments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ProductComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
