using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ContactsManager.Models
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> People { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("People");

            // Fluent API
            modelBuilder.Entity<Person>().Property(p => p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue(null);

            modelBuilder.Entity<Person>().ToTable(p => p.HasCheckConstraint("CHK_TIN", "TaxIdentificationNumber IS NULL OR LEN([TaxIdentificationNumber]) = 8"));

            // Seed data to Countries and People
            List<Country> countries = ExtractCountriesFormJson();
            List<Person> people = ExtractPeopleFormJson();

            foreach (Country country in countries) modelBuilder.Entity<Country>().HasData(country);
            foreach (Person person in people) modelBuilder.Entity<Person>().HasData(person);
        }

        public List<Person> sp_GetPeople()
        {
            return People.FromSqlRaw("EXECUTE [dbo].[GetPeople]").ToList();
        }

        public void sp_AddPerson(Person person)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PersonId", person.PersonId),
                new SqlParameter("@PersonName", person.PersonName is not null ? person.PersonName : DBNull.Value),
                new SqlParameter("@PersonEmail", person.PersonEmail is not null ? person.PersonEmail : DBNull.Value),
                new SqlParameter("@DateOfBirth", person.DateOfBirth is not null ? person.DateOfBirth : DBNull.Value),
                new SqlParameter("@Gender", person.Gender is not null ? person.Gender : DBNull.Value),
                new SqlParameter("@CountryId", person.CountryId is not null ? person.CountryId : DBNull.Value),
                new SqlParameter("@Address", person.Address is not null ? person.Address : DBNull.Value),
                new SqlParameter("@IsReceivingNewsLetters", person.IsReceivingNewsLetters)
            };
            Database.ExecuteSqlRaw("EXECUTE [dbo].[AddPerson] @PersonId, @PersonName, @PersonEmail, @DateOfBirth, @Gender, @CountryId, @Address, @IsReceivingNewsLetters", sqlParameters);
        }

        private List<Country> ExtractCountriesFormJson()
        {
            string path = Path.Combine();
            string countriesJson = File.ReadAllText("wwwroot/seed-data/countries.json");
            return JsonSerializer.Deserialize<List<Country>>(countriesJson)!;
        }

        private List<Person> ExtractPeopleFormJson()
        {
            string peopleJson = File.ReadAllText("wwwroot/seed-data/people.json");
            return JsonSerializer.Deserialize<List<Person>>(peopleJson)!;
        }
    }
}
