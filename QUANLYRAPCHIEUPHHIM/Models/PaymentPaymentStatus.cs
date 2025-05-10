using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class PaymentPaymentStatus
{
    public int PaymentPaymentStatusId { get; set; }

    public int PaymentId { get; set; }

    public int PaymentStatusId { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;
}
