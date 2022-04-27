using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WebBankingApp.Models
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40), JsonProperty("City")]
        public string Suburb { get; set; }

        [StringLength(3)]
        public string State { get; set; }

        [StringLength(12)]
        public string Mobile { get; set; }

        [StringLength(4)]
        public string PostCode { get; set; }

        [StringLength(11)]
        public string TFN { get; set; }

        public virtual List<Account> Accounts { get; set; }

        //public Customer(int customerID, string name, string address, string suburb, string postCode)
        //{
        //    CustomerID = customerID;
        //    Name = name;
        //    if (address == null)
        //    {
        //        Address = "NULL";
        //    }
        //    else
        //    {
        //        Address = address;
        //    }
        //    if (suburb == null)
        //    {
        //        Suburb = "NULL";
        //    }
        //    else
        //    {
        //        Suburb = suburb;
        //    }
        //    if (postCode == null)
        //    {
        //        PostCode = "NULL";
        //    }
        //    else
        //    {
        //        PostCode = postCode;
        //    }
        //}
    }
}

