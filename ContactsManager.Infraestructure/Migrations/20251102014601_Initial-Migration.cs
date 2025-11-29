using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContactsManager.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PersonEmail = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsReceivingNewsLetters = table.Column<bool>(type: "bit", nullable: false),
                    TaxIdentificationNumber = table.Column<string>(type: "varchar(8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonId);
                    table.CheckConstraint("CHK_TIN", "TaxIdentificationNumber IS NULL OR LEN([TaxIdentificationNumber]) = 8");
                    table.ForeignKey(
                        name: "FK_People_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[,]
                {
                    { new Guid("13c250b3-2af6-40a9-9616-6e5abf1ea2a9"), "Costa Rica" },
                    { new Guid("6e3be723-13e2-4068-b392-d9353ed41f6d"), "Germany" },
                    { new Guid("969874c5-a77d-4158-b033-605ff940d04e"), "Chile" },
                    { new Guid("d67524d3-bf47-4f48-aef1-e20bbe3f0443"), "Ireland" },
                    { new Guid("fdde1f72-2a86-4a39-9ecc-846d840b91b9"), "Canada" }
                });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "PersonId", "Address", "CountryId", "DateOfBirth", "Gender", "IsReceivingNewsLetters", "PersonEmail", "PersonName", "TaxIdentificationNumber" },
                values: new object[,]
                {
                    { new Guid("0eae4fe4-e774-4679-93d2-1da948d25dcc"), "96 Welch Street", new Guid("fdde1f72-2a86-4a39-9ecc-846d840b91b9"), new DateTime(2003, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", false, "hmora6@google.ru", "Hattie", null },
                    { new Guid("10b999ce-d7cd-48ed-a795-4bfe2b9c47d3"), "802 Melody Alley", new Guid("969874c5-a77d-4158-b033-605ff940d04e"), new DateTime(1990, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", false, "lcrittal7@dmoz.org", "Lindsy", null },
                    { new Guid("14c014a7-27ea-42b7-914d-5b6a8c827660"), "09 Dwight Point", new Guid("6e3be723-13e2-4068-b392-d9353ed41f6d"), new DateTime(2002, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", false, "jpodbury8@usatoday.com", "Jeffry", null },
                    { new Guid("254d8111-bf36-422c-a601-910d8cd3b7bd"), "677 Nelson Junction", new Guid("d67524d3-bf47-4f48-aef1-e20bbe3f0443"), new DateTime(2004, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", true, "sespinha2@auda.org.au", "Sayre", null },
                    { new Guid("34df3927-b48d-46d0-b515-58429145006f"), "53019 Daystar Plaza", new Guid("d67524d3-bf47-4f48-aef1-e20bbe3f0443"), new DateTime(1980, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", false, "dslimm5@miitbeian.gov.cn", "Demetris", null },
                    { new Guid("96d243ca-e71a-464c-8f7a-89d81dc60c2b"), "9 Dakota Place", new Guid("969874c5-a77d-4158-b033-605ff940d04e"), new DateTime(1987, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", true, "agarlee4@yolasite.com", "Allyn", null },
                    { new Guid("a2ef3bec-5e7f-40a7-ae9f-8adfba33e515"), "681 Sutherland Road", new Guid("6e3be723-13e2-4068-b392-d9353ed41f6d"), new DateTime(2022, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", false, "alukesch0@networksolutions.com", "Alfy", null },
                    { new Guid("ccf78aad-0832-480e-8009-1190ae9f0445"), "418 Veith Junction", new Guid("fdde1f72-2a86-4a39-9ecc-846d840b91b9"), new DateTime(1997, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", true, "cmatuschek9@microsoft.com", "Chaim", null },
                    { new Guid("f530dd6d-175c-4c1c-8574-a2bbbae1fc2d"), "135 Clove Crossing", new Guid("969874c5-a77d-4158-b033-605ff940d04e"), new DateTime(1992, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", false, "bferrolli3@tiny.cc", "Britt", null },
                    { new Guid("f87d9a0d-dc9b-4751-9d0a-274bd26c1acb"), "0 Springs Hill", new Guid("13c250b3-2af6-40a9-9616-6e5abf1ea2a9"), new DateTime(2010, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", false, "kcraft1@dagondesign.com", "Kevyb", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_People_CountryId",
                table: "People",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
