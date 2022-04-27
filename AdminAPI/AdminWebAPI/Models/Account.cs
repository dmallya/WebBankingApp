using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

public enum AccountType
{
    Checking = 'C',
    Saving = 'S'
}

public class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Type")]
    public string AccountType { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    //public virtual Customer Customer { get; set; }

    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

    public virtual List<BillPay> BillPays { get; set; }

    public virtual List<Transaction> Transactions { get; set; }

}

