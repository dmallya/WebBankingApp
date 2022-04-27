using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBankingApp.Models
{
    public enum PeriodType
    {
        OneOff = 'O',
        Monthly = 'M',
        Blocked = 'B',
        Failed = 'F',
        Paid = 'P',
    }


    public class BillPay
    {
        public int BillPayId { get; set; }

        [ForeignKey("Account")]
        public int AccountNumber { get; set; }

        [ForeignKey("Payee")]
        public int PayeeID { get; set; }

        [Column(TypeName = "decimal")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        public DateTime ScheduleTimeUtc { get; set; }
        public PeriodType Period { get; set; }
        public DateTime LastPaid { get; set; }
        public virtual Account Account { get; set; }
        public virtual Payee Payee { get; set; }

    }
}