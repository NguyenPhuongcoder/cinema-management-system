using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetPaymentsAsync(
            int? bookingId = null,
            int? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? paymentMethodId = null,
            int page = 1,
            int pageSize = 10);

        Task<Payment> GetPaymentByIdAsync(int id);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task<bool> DeletePaymentAsync(int id);
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
        Task<PaymentMethod> GetPaymentMethodByIdAsync(int id);
    }
} 