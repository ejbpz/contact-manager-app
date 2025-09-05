using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ContactsManager.Models
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> People { get; set; }

        public PeopleDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("People");

            // Seed data to Countries and People
            List<Country> countries = ExtractCountriesFormJson();
            List<Person> people = ExtractPeopleFormJson();

            foreach (Country country in countries) modelBuilder.Entity<Country>().HasData(country);
            foreach (Person person in people) modelBuilder.Entity<Person>().HasData(person);
        }

        private List<Country> ExtractCountriesFormJson()
        {
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
