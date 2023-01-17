using System.ComponentModel.DataAnnotations;

namespace Rabobank.TechnicalTest.GCOB.Domain
{
    public class Customer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}
