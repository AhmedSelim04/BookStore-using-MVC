using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditOrederTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingTime",
                table: "OrderHeaders",
                newName: "ShippingDate");

            migrationBuilder.RenameColumn(
                name: "OrederTotal",
                table: "OrderHeaders",
                newName: "OrderTotal");

            migrationBuilder.RenameColumn(
                name: "OrderTime",
                table: "OrderHeaders",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "OrderDetails",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingDate",
                table: "OrderHeaders",
                newName: "ShippingTime");

            migrationBuilder.RenameColumn(
                name: "OrderTotal",
                table: "OrderHeaders",
                newName: "OrederTotal");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "OrderHeaders",
                newName: "OrderTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderDetails",
                newName: "id");
        }
    }
}
