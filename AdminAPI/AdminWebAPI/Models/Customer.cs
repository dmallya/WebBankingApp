using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AdminWebAPI.Models
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Address { get; set; }

        [StringLength(40), JsonProperty("City")]
        public string? Suburb { get; set; }

        [StringLength(3)]
        public string? State { get; set; }

        [StringLength(12)]
        public string? Mobile { get; set; }

        [StringLength(4)]
        public string? PostCode { get; set; }

        [StringLength(11)]
        public string? TFN { get; set; }

        public virtual List<Account>? Accounts { get; set; }

    }
}

