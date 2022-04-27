using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models
{
    public enum TransactionType
    {
        Deposit = 'D',
        Withdrawal = 'W',
        Transfer = 'T',
        Service = 'S',
        BillPay = 'B'
    }

    public class Transaction
    {
        public int TransactionID { get; set; }
        public TransactionType TransactionType { get; set; }

        //[ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        //public virtual Account DestinationAccount { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string? Comment { get; set; }
        public DateTime TransactionTimeUtc { get; set; }
    }
}