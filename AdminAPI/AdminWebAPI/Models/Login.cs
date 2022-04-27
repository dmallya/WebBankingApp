using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

public enum Authorization
{
    Authorized = 'A',
    Unauthorized = 'U',
}

public class Login
{
    [Column(TypeName = "char")]
    [StringLength(8)]
    public string LoginID { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    //public virtual Customer Customer { get; set; }

    [Column(TypeName = "char")]
    [Required, StringLength(64)]
    public string PasswordHash { get; set; }
    public Authorization Auth { get; set; }
}
