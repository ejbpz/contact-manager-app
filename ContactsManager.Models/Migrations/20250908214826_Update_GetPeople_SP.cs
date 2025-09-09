using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Models.Migrations
{
    /// <inheritdoc />
    public partial class Update_GetPeople_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetPeople = @"
                ALTER PROCEDURE [dbo].[GetPeople]
                AS BEGIN
                   SELECT PersonId, PersonName, PersonEmail, DateOfBirth, Gender, CountryId, Address, IsReceivingNewsLetters, TaxIdentificationNumber FROM [dbo].[People]
                END
            ";
            migrationBuilder.Sql(sp_GetPeople);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetPeople = @"
                DROP PROCEDURE [dbo].[GetPeople]
            ";
            migrationBuilder.Sql(sp_GetPeople);
        }
    }
}
