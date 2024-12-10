

namespace SellersZone.Core.Enums
{
    public enum OrderStatus
    {
        OnRequest = 1, // قيد الطلب
        UnderConfirmation = 2, // قيد التأكيد من الزبون
        UnderDelivery = 3, // قيد التوصيل
        Delivered = 4, // تم التوصيل
        Canceled = 5, // ملغي 
        Returned = 6 // مرجع
    }

    public enum DeliveryReturnPayer
    {
        Customer = 1,
        Seller = 2,
        Us = 3
    }

    public enum PurchaseProductStatus
    {
        OnRequest = 1,
        Approved = 2,
        Reject = 3
    }

    public enum SideEarningStatus
    {
        OnPending = 1,
        Transfer = 2,
        Rollack = 3
    }
}
