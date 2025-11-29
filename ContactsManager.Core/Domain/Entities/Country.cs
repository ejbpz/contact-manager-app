using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Models
{
    /// <summary>
    /// Domain Model for Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }

        [StringLength(50)]
        public string? CountryName { get; set; }
    }
}
