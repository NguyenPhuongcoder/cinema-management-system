using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
