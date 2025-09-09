using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsManager.Models
{
    /// <summary>
    /// Person domain model class.
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }

        [StringLength(40)] // nvarchar(40)
        public string? PersonName { get; set; }

        [StringLength(40)]
        public string? PersonEmail { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        // Unique Identifier
        public Guid? CountryId { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        // bit in SQL
        public bool IsReceivingNewsLetters { get; set; }

        // Tax Identification Number
        public string? TIN { get; set; }

        [ForeignKey("CountryId")]
        public Country? Country { get; set; }
    }
}
