using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Models.Migrations
{
    /// <inheritdoc />
    public partial class New_TIN_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxIdentificationNumber",
                table: "People",
                type: "varchar(8)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("0eae4fe4-e774-4679-93d2-1da948d25dcc"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("10b999ce-d7cd-48ed-a795-4bfe2b9c47d3"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("14c014a7-27ea-42b7-914d-5b6a8c827660"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("254d8111-bf36-422c-a601-910d8cd3b7bd"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("34df3927-b48d-46d0-b515-58429145006f"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("96d243ca-e71a-464c-8f7a-89d81dc60c2b"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("a2ef3bec-5e7f-40a7-ae9f-8adfba33e515"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("ccf78aad-0832-480e-8009-1190ae9f0445"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("f530dd6d-175c-4c1c-8574-a2bbbae1fc2d"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: new Guid("f87d9a0d-dc9b-4751-9d0a-274bd26c1acb"),
                column: "TaxIdentificationNumber",
                value: null);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TIN",
                table: "People",
                sql: "TaxIdentificationNumber IS NULL OR LEN([TaxIdentificationNumber]) = 8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_TIN",
                table: "People");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationNumber",
                table: "People");
        }
    }
}
