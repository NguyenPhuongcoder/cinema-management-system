using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class PaymentStatus
{
    public int PaymentStatusId { get; set; }

    public string PaymentStatusName { get; set; } = null!;

    public virtual ICollection<PaymentPaymentStatus> PaymentPaymentStatuses { get; set; } = new List<PaymentPaymentStatus>();
}
