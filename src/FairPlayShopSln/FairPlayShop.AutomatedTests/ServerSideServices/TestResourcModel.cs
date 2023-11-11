using System.ComponentModel.DataAnnotations;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    internal class TestResourcModel
    {
        public int ResourceId { get; set; }

        [Required]
        [StringLength(1500)]
        public string? Type { get; set; }

        [Required]
        [StringLength(50)]
        public string? Key { get; set; }

        [Required]
        public string? Value { get; set; }
        public int CultureId { get; set; }
    }
}