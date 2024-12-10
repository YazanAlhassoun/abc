using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;

namespace SellersZone.Core.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Response<PaymentMethodDto> GetPaymentMethod(RequestParam param);
        Response<PaymentMethodDto> AddPaymentMethod(PaymentMethodDto paymentMethodDto);
        Response<PaymentMethodDto> UpdatePaymentMethod(PaymentMethodDto paymentMethodDto);
    }
}
