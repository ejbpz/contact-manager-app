using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_AddPerson = @"
                CREATE PROCEDURE [dbo].[AddPerson] (@PersonId uniqueidentifier, @PersonName nvarchar(40), @PersonEmail nvarchar(40), @DateOfBirth datetime2(7), @Gender nvarchar(10), @CountryId uniqueidentifier, @Address nvarchar(200), @IsReceivingNewsLetters bit)
                AS BEGIN
                    INSERT INTO [dbo].[People] (PersonId, PersonName, PersonEmail, DateOfBirth, Gender, CountryId, Address, IsReceivingNewsLetters) VALUES (@PersonId, @PersonName, @PersonEmail, @DateOfBirth, @Gender, @CountryId, @Address, @IsReceivingNewsLetters)
                END
            ";
            migrationBuilder.Sql(sp_AddPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_AddPerson = @"
                DROP PROCEDURE [dbo].[AddPerson]
            ";
            migrationBuilder.Sql(sp_AddPerson);
        }
    }
}
